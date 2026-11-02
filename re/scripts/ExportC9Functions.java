// Export specified functions, assembly, callers and callees for manual verification.
// @category C9Research
import java.io.*;
import java.nio.charset.StandardCharsets;
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.*;

public class ExportC9Functions extends GhidraScript {
    public void run() throws Exception {
        String[] args = getScriptArgs();
        DecompInterface decomp = new DecompInterface();
        if (!decomp.openProgram(currentProgram)) throw new IOException("Decompiler initialization failed");
        try (PrintWriter out = new PrintWriter(new File(args[0] + ".c"), StandardCharsets.UTF_8);
             PrintWriter asm = new PrintWriter(new File(args[0] + ".asm.txt"), StandardCharsets.UTF_8)) {
            out.println("/* SHA256 " + currentProgram.getExecutableSHA256() + " */");
            for (int i = 1; i < args.length; i++) {
                monitor.checkCancelled();
                Function f = getFunctionContaining(toAddr(args[i]));
                if (f == null) {
                    out.println("/* No defined function at " + args[i] + " */");
                    continue;
                }
                out.println("\n/* " + f.getEntryPoint() + " " + f.getName() + " */");
                out.println("/* Callers:");
                for (Reference r : getReferencesTo(f.getEntryPoint())) {
                    Function caller = getFunctionContaining(r.getFromAddress());
                    out.println(" * " + r.getFromAddress() + " " +
                        (caller == null ? "data reference" : caller.getEntryPoint() + " " + caller.getName()));
                }
                out.println(" * Callees:");
                for (Function callee : f.getCalledFunctions(monitor))
                    out.println(" * " + callee.getEntryPoint() + " " + callee.getName());
                out.println(" */");
                DecompileResults result = decomp.decompileFunction(f, 20, monitor);
                if (result.decompileCompleted()) out.println(result.getDecompiledFunction().getC());
                else out.println("/* Unavailable: " + result.getErrorMessage() + " */");
                asm.println("\n; " + f.getEntryPoint() + " " + f.getName());
                InstructionIterator instructions = currentProgram.getListing().getInstructions(f.getBody(), true);
                while (instructions.hasNext()) {
                    Instruction ins = instructions.next();
                    asm.println(ins.getAddress() + " " + ins);
                }
            }
        } finally { decomp.dispose(); }
        println("C9_FUNCTION_EXPORT_COMPLETE " + args[0]);
    }
}
