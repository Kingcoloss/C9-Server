// Define explicitly selected x86 functions in the separate research database.
// @category C9Research
import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.Function;

public class RecoverC9Functions extends GhidraScript {
    public void run() throws Exception {
        String[] args = getScriptArgs();
        for (int i = 1; i < args.length; i++) {
            monitor.checkCancelled();
            Address address = toAddr(args[i]);
            Function existing = getFunctionContaining(address);
            if (existing != null) {
                if (!existing.getEntryPoint().equals(address))
                    throw new IllegalArgumentException("Address inside another function: " + address);
                continue;
            }
            if (getInstructionAt(address) == null && !disassemble(address))
                throw new IllegalStateException("Disassembly failed: " + address);
            if (createFunction(address, null) == null)
                throw new IllegalStateException("Function creation failed: " + address);
        }
        runScript("ExportC9Functions.java", args);
    }
}
