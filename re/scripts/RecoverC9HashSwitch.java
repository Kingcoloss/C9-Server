// Recover code reached through the bounded hash-selection jump table.
// @category C9Research
import ghidra.app.script.GhidraScript;
import ghidra.app.cmd.function.CreateFunctionCmd;
import ghidra.program.model.address.Address;
import ghidra.program.model.listing.Function;
import ghidra.program.model.symbol.RefType;
import ghidra.program.model.symbol.SourceType;

public class RecoverC9HashSwitch extends GhidraScript {
    public void run() throws Exception {
        Address table = toAddr("00dd5a30");
        Address branch = toAddr("00dd5513");
        for (int i = 0; i < 10; i++) {
            Address target = toAddr(Integer.toUnsignedLong(getInt(table.add(i * 4))));
            if (getInstructionAt(target) == null && !disassemble(target))
                throw new IllegalStateException("Cannot disassemble " + target);
            currentProgram.getReferenceManager().addMemoryReference(branch, target,
                RefType.COMPUTED_JUMP, SourceType.USER_DEFINED, 0);
        }
        Function f = getFunctionAt(toAddr("00dd5470"));
        if (!CreateFunctionCmd.fixupFunctionBody(currentProgram, f, monitor))
            throw new IllegalStateException("Hash function body repair failed");
        runScript("ExportC9Functions.java", new String[]{getScriptArgs()[0], "00dd5470"});
    }
}
