// Enumerate defined functions in a bounded address range and export them statically.
// @category C9Research
import java.util.*;
import ghidra.app.script.GhidraScript;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;

public class ExportC9Range extends GhidraScript {
    public void run() throws Exception {
        String[] args = getScriptArgs();
        Address start = toAddr(args[1]);
        Address end = toAddr(args[2]);
        ArrayList<String> exportArgs = new ArrayList<>();
        exportArgs.add(args[0]);
        FunctionIterator iterator = currentProgram.getFunctionManager().getFunctions(start, true);
        while (iterator.hasNext() && exportArgs.size() <= 64) {
            Function f = iterator.next();
            if (f.getEntryPoint().compareTo(end) > 0) break;
            exportArgs.add(f.getEntryPoint().toString());
        }
        runScript("ExportC9Functions.java", exportArgs.toArray(new String[0]));
        println("C9_RANGE_EXPORT_COMPLETE functions=" + (exportArgs.size() - 1));
    }
}
