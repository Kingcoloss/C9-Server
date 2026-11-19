// Emit synthetic test vectors from isolated x86 payload transforms using p-code only.
// No process, network, imports, or entry point of the reference program is executed.
// @category C9Research
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.*;
import java.util.*;
import ghidra.app.script.GhidraScript;
import ghidra.app.emulator.EmulatorHelper;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.Instruction;

public class ExportC9TwisterVectors extends GhidraScript {
    private static final long BUFFER = 0x70000000L, STACK = 0x71000000L, RETURN = 0x72000000L;
    private String hex(byte[] bytes) { return HexFormat.of().formatHex(bytes); }

    private byte[] transform(EmulatorHelper emu, long function, byte[] input) throws Exception {
        byte[] guarded = new byte[input.length + 128];
        Arrays.fill(guarded, (byte)0xA5);
        System.arraycopy(input, 0, guarded, 64, input.length);
        emu.writeMemory(toAddr(BUFFER - 64), guarded);
        emu.writeRegister("ESP", STACK);
        emu.writeRegister("ECX", 0);
        emu.writeRegister("EFLAGS", 0x202);
        emu.writeMemoryValue(toAddr(STACK), 4, RETURN);
        emu.writeMemoryValue(toAddr(STACK + 4), 4, BUFFER);
        emu.writeMemoryValue(toAddr(STACK + 8), 4, input.length);
        emu.writeRegister("EIP", function);
        for (int steps = 0; steps < 5000; steps++) {
            monitor.checkCancelled();
            long pc = emu.getExecutionAddress().getOffset();
            if (pc == RETURN) {
                byte[] actual = emu.readMemory(toAddr(BUFFER - 64), guarded.length);
                for (int i = 0; i < 64; i++)
                    if (actual[i] != (byte)0xA5 || actual[actual.length - 1 - i] != (byte)0xA5)
                        throw new IOException("Transform exceeded payload at " + Long.toHexString(function));
                return Arrays.copyOfRange(actual, 64, 64 + input.length);
            }
            if (pc < function || pc >= function + 0x3000)
                throw new IOException("Transform escaped selected routine: " + Long.toHexString(pc));
            Instruction ins = getInstructionAt(toAddr(pc));
            if (ins != null && ins.getFlowType().isCall())
                throw new IOException("Transform calls another routine: " + Long.toHexString(pc));
            if (!emu.step(monitor)) throw new IOException(emu.getLastError());
        }
        throw new IOException("Transform exceeded instruction budget");
    }

    public void run() throws Exception {
        String[] args = getScriptArgs();
        if (!currentProgram.getExecutableSHA256().equals("567d2f38066a908d0276f3d44fbdb808ae5748b2adde57446d44d3cfac9ffb51"))
            throw new IOException("This experiment requires the pinned Ref 2014 GameServer");
        int[] sizes = {0,1,3,4,7,8,15,16,31,32,33,46,256};
        Random random = new Random(0xC92014);
        int count = 0;
        try (PrintWriter out = new PrintWriter(args[1], StandardCharsets.UTF_8)) {
            out.println("{\"source_sha256\":\"" + currentProgram.getExecutableSHA256() + "\",\"method\":\"bounded Ghidra p-code emulation of pure payload transforms\",\"vectors\":[");
            for (String row : Files.readAllLines(Path.of(args[0]), StandardCharsets.UTF_8)) {
                String[] cols = row.split("\\t");
                if (cols.length != 3) throw new IOException("Expected selector / encode VA / decode VA");
                int selector = Integer.parseInt(cols[0]);
                for (int direction = 0; direction < 2; direction++) {
                    long address = Long.parseLong(cols[direction + 1], 16);
                    if (getInstructionAt(toAddr(address)) == null && !disassemble(toAddr(address)))
                        throw new IOException("Cannot disassemble selected transform");
                    EmulatorHelper emu = new EmulatorHelper(currentProgram);
                    try {
                        for (int size : sizes) {
                            for (int sample = 0; sample < 2; sample++) {
                                byte[] input = new byte[size];
                                if (sample == 1) random.nextBytes(input);
                                byte[] output = transform(emu, address, input);
                                if (count++ > 0) out.println(",");
                                out.print("{\"selector\":" + selector + ",\"direction\":\"" +
                                    (direction == 0 ? "encode" : "decode") + "\",\"source_va\":\"" + cols[direction + 1] +
                                    "\",\"input_hex\":\"" + hex(input) + "\",\"output_hex\":\"" + hex(output) + "\"}");
                            }
                        }
                    } finally { emu.dispose(); }
                }
                out.flush();
                println("C9_REF_TWISTER_SELECTOR_COMPLETE " + selector);
            }
            out.println("\n],\"count\":" + count + "}");
        }
        println("C9_REF_TWISTER_VECTORS_COMPLETE " + count);
    }
}
