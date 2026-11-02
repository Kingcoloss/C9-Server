"""Portable payload transforms recovered from the observed modulo-80 registry."""
import json
from functools import lru_cache
from pathlib import Path


@lru_cache(maxsize=1)
def _tables():
    source = Path(__file__).with_name('data') / 'twisters.json'
    catalog = json.loads(source.read_text())
    if len(catalog['entries']) != 80 or catalog['selector_modulus'] != 80:
        raise RuntimeError('Incomplete PacketTwister tables')
    return catalog['entries']


def transform(payload, selector, *, decode=False):
    payload = bytes(payload)
    size = len(payload)
    if size < 4:
        return payload
    branch = 0 if size < 8 else 1 if size < 16 else 2 if size < 32 else 3
    spec = _tables()[selector % 80]['decode' if decode else 'encode']['length_branches'][branch]
    width = spec['width']
    original = int.from_bytes(payload[:width], 'little')
    result = original
    for destination, source in spec['moves']:
        result = (result & ~(1 << destination)) | (((original >> source) & 1) << destination)
    result ^= int(spec['xor_hex'], 16)
    return result.to_bytes(width, 'little') + payload[width:]
