// Reference model of the client cipher-epoch logic (client b4580ed4, 00D04730..00D0A880),
// built on Crypto++ (the library the client links) to produce vectors for the C# port.
// Build (Ubuntu 24.04, libcrypto++-dev 8.9):  g++ -O2 -std=c++17 -o ref ref.cpp -lcrypto++
//   ./ref > tests/fixtures/cipher_epoch_vectors.json
//   ./ref tables > tables.json   (source of src/C9.Protocol/LegacyCipherTables.cs)
#include <cstdio>
#include <cstdint>
#include <cstring>
#include <string>
#include <vector>
#include <memory>
#include <sstream>
#include <iostream>
#include <map>
#include <set>
#include <stdexcept>
#define CRYPTOPP_ENABLE_NAMESPACE_WEAK 1
#include <crypto++/sha.h>
#include <crypto++/md5.h>
#include <crypto++/md4.h>
#include <crypto++/ripemd.h>
#include <crypto++/tea.h>
#include <crypto++/3way.h>
#include <crypto++/cast.h>
#include <crypto++/gost.h>
#include <crypto++/rc5.h>
#include <crypto++/rc6.h>
#include <crypto++/mars.h>
#include <crypto++/idea.h>
#include <crypto++/twofish.h>
#include <crypto++/des.h>
#include <crypto++/skipjack.h>
#include <crypto++/safer.h>
extern const uint32_t t_cast_s[8][256] __asm__("_ZN8CryptoPP4CAST1SE");
extern const uint8_t t_gost_sbox[8][16] __asm__("_ZN8CryptoPP4GOST4Base4sBoxE");
extern const uint32_t t_mars_sbox[512] __asm__("_ZN8CryptoPP4MARS4Base4SboxE");
extern const uint8_t t_safer_exp[256] __asm__("_ZN8CryptoPP5SAFER4Base7exp_tabE");
extern const uint8_t t_safer_log[256] __asm__("_ZN8CryptoPP5SAFER4Base7log_tabE");
extern const uint32_t t_des_spbox[8][64] __asm__("_ZN8CryptoPP6RawDES5SpboxE");
extern const uint8_t t_twofish_q[2][256] __asm__("_ZN8CryptoPP7Twofish4Base1qE");
extern const uint32_t t_twofish_mds[4][256] __asm__("_ZN8CryptoPP7Twofish4Base3mdsE");
extern const uint8_t t_skipjack_f[256] __asm__("_ZN8CryptoPP8SKIPJACK4Base6fTableE");
using namespace CryptoPP;
using Bytes = std::vector<uint8_t>;

static std::string hex(const uint8_t* p, size_t n){ static const char* h="0123456789abcdef"; std::string s; for(size_t i=0;i<n;i++){s+=h[p[i]>>4]; s+=h[p[i]&15];} return s; }
static std::string hex(const Bytes& b){ return hex(b.data(), b.size()); }

struct Rng { uint64_t s; explicit Rng(uint64_t seed):s(seed){} uint32_t next(){ s ^= s << 13; s ^= s >> 7; s ^= s << 17; return (uint32_t)(s >> 32); } uint8_t byte(){ return (uint8_t)next(); } };

struct Cipher {
  std::string name; unsigned blockSize, keyLength, threshold;
  std::unique_ptr<BlockTransformation> enc, dec;
};
template<class C> static Cipher make(const char* name, unsigned keyLength, unsigned threshold, const uint8_t* key) {
  Cipher c; c.name=name; c.keyLength=keyLength; c.threshold=threshold;
  auto e = new typename C::Encryption(); e->SetKey(key, keyLength, g_nullNameValuePairs);
  auto d = new typename C::Decryption(); d->SetKey(key, keyLength, g_nullNameValuePairs);
  c.blockSize = e->BlockSize(); c.enc.reset(e); c.dec.reset(d); return c;
}
// 00D0A480 + ConcreteCipher<...>::init (key length, threshold)
static Cipher makeCipher(unsigned index, const uint8_t* key) {
  switch (index % 10) {
    case 0: return make<TEA>("TEA", 16, 256, key);
    case 1: return make<ThreeWay>("ThreeWay", 12, 256, key);
    case 2: return make<CAST128>("CAST128", 16, 256, key);
    case 4: return make<RC5>("RC5", 16, 256, key);
    case 5: return make<RC6>("RC6", 16, 256, key);
    case 6: return make<MARS>("MARS", 16, 256, key);
    case 7: return make<IDEA>("IDEA", 16, 256, key);
    case 9:
      switch (key[0] & 7) {
        case 0: return make<Twofish>("Twofish", 16, 8, key);
        case 1: return make<DES>("DES", 8, 8, key);
        case 2: return make<DES_EDE2>("DES_EDE2", 16, 8, key);
        case 3: return make<DES_EDE3>("DES_EDE3", 24, 32, key);
        case 4: return make<DES_XEX3>("DES_XEX3", 24, 8, key);
        case 5: return make<SKIPJACK>("SKIPJACK", 10, 32, key);
        case 6: return make<SAFER_K>("SAFER_K", 16, 8, key);
        default: return make<SAFER_SK>("SAFER_SK", 16, 32, key);
      }
    default: return make<GOST>("GOST", 32, 256, key); // 3 and 8
  }
}
static std::unique_ptr<HashTransformation> makeHash(unsigned c, std::string& name, unsigned& at) {
  switch (c) {
    case 1: name="SHA256"; at=0; return std::make_unique<SHA256>();
    case 3: name="SHA512"; at=1; return std::make_unique<SHA512>();
    case 4: name="SHA384"; at=2; return std::make_unique<SHA384>();
    case 5: name="MD4"; at=3; return std::make_unique<Weak::MD4>();
    case 6: name="MD5"; at=4; return std::make_unique<Weak::MD5>();
    case 7: name="RIPEMD160"; at=5; return std::make_unique<RIPEMD160>();
    case 8: name="RIPEMD320"; at=6; return std::make_unique<RIPEMD320>();
    case 9: name="RIPEMD128"; at=7; return std::make_unique<RIPEMD128>();
    case 10: name="RIPEMD256"; at=8; return std::make_unique<RIPEMD256>();
    default: name="MD5"; at=9; return std::make_unique<Weak::MD5>(); // 0 and 2
  }
}
struct Selection { unsigned selector, hashCase, cipherIndex; std::string hash; Bytes digest; };
// 00D0A880
static Cipher derive(const uint8_t* seed, size_t len, Selection& sel) {
  uint8_t s;
  if (len < 10) s = seed[0];
  else { uint8_t a = seed[1] ^ seed[0]; a = (uint8_t)(a << (seed[2] % 3)); s = (uint8_t)((a | seed[3]) ^ seed[4]); }
  sel.selector = s; sel.hashCase = s % 11;
  unsigned at; auto h = makeHash(sel.hashCase, sel.hash, at);
  uint8_t buf[0x800]; memset(buf, 0, sizeof buf);
  h->Update(seed, len); h->TruncatedFinal(buf, h->DigestSize());
  sel.digest.assign(buf, buf + h->DigestSize());
  sel.cipherIndex = buf[at] % 10;
  return makeCipher(sel.cipherIndex, buf);
}
struct Direction {
  Cipher cipher; unsigned counter = 1; uint8_t table[128]; std::vector<std::string> epochs;
  Direction(const uint8_t* seed, size_t len) { memset(table, 99, 128); Selection s; cipher = derive(seed, len, s); epochs.push_back(cipher.name); }
  void commit(const uint8_t* p, size_t n) { size_t c = n > 8 ? 8 : (n ? 1 : 0); for (size_t i=0;i<c;i++) table[p[i] & 0x7f] = p[i]; }
  void step(const uint8_t* plain, size_t n) {
    commit(plain, n);
    if (counter == cipher.threshold) { Selection s; cipher = derive(table, 128, s); epochs.push_back(cipher.name); counter = 1; }
    else counter++;
  }
  Bytes encrypt(const Bytes& plain) { // ConcreteCipher vtable+8 (00D06610 / 00D07D80 / 00D084B0)
    unsigned bs = cipher.blockSize; size_t pad = plain.size() % bs ? bs - plain.size() % bs : 0;
    Bytes buf(plain); buf.resize(plain.size() + pad, 0); Bytes out(buf.size() + 1);
    for (size_t i=0;i<buf.size();i+=bs) cipher.enc->ProcessAndXorBlock(&buf[i], nullptr, &out[i]);
    out.back() = (uint8_t)pad; step(plain.data(), plain.size()); return out;
  }
  Bytes decrypt(const Bytes& wire) {
    unsigned bs = cipher.blockSize; if (wire.size() % bs != 1) throw std::runtime_error("len");
    size_t pad = wire.back(); size_t n = wire.size() - 1 - pad; Bytes out(wire.size() - 1);
    for (size_t i=0;i<n;i+=bs) cipher.dec->ProcessAndXorBlock(&wire[i], nullptr, &out[i]);
    out.resize(n); step(out.data(), n); return out;
  }
};
template<class T, size_t N> static std::string arr(const T (&a)[N]) { std::ostringstream o; o << "["; for (size_t i=0;i<N;i++){ if(i) o<<","; o<<(uint64_t)a[i]; } o << "]"; return o.str(); }
template<class T, size_t M, size_t N> static std::string arr2(const T (&a)[M][N]) { std::ostringstream o; o << "["; for (size_t i=0;i<M;i++){ if(i) o<<","; o<<arr(a[i]); } o << "]"; return o.str(); }

int main(int argc, char** argv) {
  std::string mode = argc > 1 ? argv[1] : "vectors";
  if (mode == "tables") {
    std::cout << "{\"cast_s\":" << arr2(t_cast_s) << ",\"mars_sbox\":" << arr(t_mars_sbox)
      << ",\"skipjack_f\":" << arr(t_skipjack_f) << ",\"twofish_q\":" << arr2(t_twofish_q)
      << ",\"twofish_mds\":" << arr2(t_twofish_mds) << ",\"des_spbox\":" << arr2(t_des_spbox)
      << ",\"gost_sbox\":" << arr2(t_gost_sbox) << ",\"safer_exp\":" << arr(t_safer_exp)
      << ",\"safer_log\":" << arr(t_safer_log) << "}\n";
    return 0;
  }
  Rng rng(0xC9C1F3E2D4B5A697ull);
  std::ostringstream o; o << "{\n\"ciphers\":[\n";
  const char* names[16]={0}; bool first=true;
  // Cipher KATs: index 0..8 plus every index-9 variant (selected by key[0] & 7).
  for (unsigned v = 0; v < 17; v++) {
    for (int rep = 0; rep < 3; rep++) {
      uint8_t key[64]; for (auto& b : key) b = rng.byte();
      unsigned index = v < 9 ? v : 9; if (v >= 9) key[0] = (uint8_t)((key[0] & ~7) | (v - 9));
      if (v == 8 && rep == 0) continue;
      Cipher c = makeCipher(index, key);
      Bytes plain(c.blockSize * 2); for (auto& b : plain) b = rng.byte();
      Bytes ct(plain.size()); for (size_t i=0;i<plain.size();i+=c.blockSize) c.enc->ProcessAndXorBlock(&plain[i], nullptr, &ct[i]);
      Bytes back(plain.size()); for (size_t i=0;i<plain.size();i+=c.blockSize) c.dec->ProcessAndXorBlock(&ct[i], nullptr, &back[i]);
      if (back != plain) throw std::runtime_error("roundtrip");
      o << (first ? "" : ",\n") << "{\"index\":" << index << ",\"name\":\"" << c.name << "\",\"block\":" << c.blockSize << ",\"keyLength\":" << c.keyLength
        << ",\"threshold\":" << c.threshold << ",\"key\":\"" << hex(key, c.keyLength) << "\",\"plain\":\"" << hex(plain) << "\",\"cipher\":\"" << hex(ct) << "\"}";
      first = false;
    }
  }
  o << "\n],\n\"hashes\":[\n"; first = true;
  for (unsigned c = 0; c < 11; c++) {
    for (size_t len : {0, 3, 64, 128, 200}) {
      Bytes m(len); for (auto& b : m) b = rng.byte();
      std::string name; unsigned at; auto h = makeHash(c, name, at); Bytes d(h->DigestSize()); h->CalculateDigest(d.data(), m.data(), m.size());
      o << (first ? "" : ",\n") << "{\"case\":" << c << ",\"name\":\"" << name << "\",\"at\":" << at << ",\"input\":\"" << hex(m) << "\",\"digest\":\"" << hex(d) << "\"}"; first = false;
    }
  }
  o << "\n],\n\"selections\":[\n"; first = true;
  std::set<std::string> seen;
  for (int i = 0; i < 4000 && seen.size() < 16; i++) {
    uint8_t t[128]; for (auto& b : t) b = rng.byte();
    Selection s; Cipher c = derive(t, 128, s);
    if (!seen.insert(c.name + std::to_string(s.hashCase)).second && i > 40) continue;
    o << (first ? "" : ",\n") << "{\"seed\":\"" << hex(t, 128) << "\",\"selector\":" << s.selector << ",\"hashCase\":" << s.hashCase << ",\"hash\":\"" << s.hash
      << "\",\"cipherIndex\":" << s.cipherIndex << ",\"cipher\":\"" << c.name << "\",\"threshold\":" << c.threshold << "}"; first = false;
  }
  // Initial seed (00D04730) must select RIPEMD160 -> RC6.
  { const char* seed = "33234567890123456789012345678912"; Selection s; Cipher c = derive((const uint8_t*)seed, 32, s);
    o << ",\n{\"seed\":\"" << hex((const uint8_t*)seed, 32) << "\",\"selector\":" << s.selector << ",\"hashCase\":" << s.hashCase << ",\"hash\":\"" << s.hash
      << "\",\"cipherIndex\":" << s.cipherIndex << ",\"cipher\":\"" << c.name << "\",\"threshold\":" << c.threshold << ",\"digest\":\"" << hex(s.digest) << "\"}"; }
  o << "\n],\n\"streams\":[\n";
  // Full direction streams: deterministic frames, client-side encrypt and server-side decrypt in lockstep.
  for (int st = 0; st < 3; st++) {
    uint64_t seed = 0x5EED0000ull + st; Rng fr(seed);
    const char* s0 = "33234567890123456789012345678912";
    Direction enc((const uint8_t*)s0, 32), dec((const uint8_t*)s0, 32);
    SHA256 acc; std::ostringstream samples; int frames = st == 2 ? 20000 : 3000; bool sf = true;
    for (int f = 1; f <= frames; f++) {
      size_t n = 10 + fr.next() % (st == 1 ? 40 : 300);
      Bytes p(n); for (auto& b : p) b = (uint8_t)fr.next();
      Bytes w = enc.encrypt(p); Bytes back = dec.decrypt(w);
      if (back != p) throw std::runtime_error("stream roundtrip");
      acc.Update(w.data(), w.size());
      if (f == 1 || f == 256 || f == 257 || f == 258 || f == frames) { samples << (sf ? "" : ",") << "{\"frame\":" << f << ",\"wire\":\"" << hex(w) << "\"}"; sf = false; }
    }
    Bytes d(32); acc.Final(d.data());
    std::map<std::string,int> counts; for (auto& e : enc.epochs) counts[e]++;
    o << (st ? ",\n" : "") << "{\"rngSeed\":" << seed << ",\"frames\":" << frames << ",\"maxExtra\":" << (st == 1 ? 40 : 300) << ",\"wireSha256\":\"" << hex(d) << "\",\"epochs\":" << enc.epochs.size()
      << ",\"firstEpochs\":["; for (size_t i=0;i<enc.epochs.size() && i<40;i++) o << (i?",":"") << "\"" << enc.epochs[i] << "\"";
    o << "],\"cipherCounts\":{"; bool cf=true; for (auto& kv : counts) { o << (cf?"":",") << "\"" << kv.first << "\":" << kv.second; cf=false; }
    o << "},\"samples\":[" << samples.str() << "]}";
  }
  o << "\n]\n}\n";
  std::cout << o.str();
  return 0;
}
