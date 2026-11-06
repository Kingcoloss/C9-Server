// Export the installed launcher's version.ini readers for reproducible schema evidence.
// @category C9Research
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.DecompInterface;
import ghidra.app.decompiler.DecompileResults;
import ghidra.program.model.listing.Data;
import ghidra.program.model.listing.DataIterator;
import ghidra.program.model.listing.Function;
import ghidra.program.model.symbol.Reference;
import java.io.File;
import java.io.PrintWriter;
import java.nio.charset.StandardCharsets;
import java.util.LinkedHashSet;
import java.util.Set;

public class ExportLauncherConfig extends GhidraScript {
    public void run() throws Exception {
        Set<Function> functions = new LinkedHashSet<>();
        File output = new File(getScriptArgs()[0]);
        output.getParentFile().mkdirs();
        try (PrintWriter writer = new PrintWriter(output, StandardCharsets.UTF_8)) {
            writer.println("/* SHA256 " + currentProgram.getExecutableSHA256() + " */");
            DataIterator data = currentProgram.getListing().getDefinedData(true);
            while (data.hasNext()) {
                Data item = data.next();
                Object value = item.getValue();
                if (!(value instanceof String)) continue;
                String text = (String)value;
                if (!(text.equals("server") || text.equals("DOWNLOAD") || text.equals("VERSION") ||
                      text.contains("obtain download urls") || text.contains("make download urls") ||
                      text.contains("localinfo.dat"))) continue;
                writer.println("// String " + item.getAddress() + ": " + text);
                for (Reference reference : getReferencesTo(item.getAddress())) {
                    Function function = getFunctionContaining(reference.getFromAddress());
                    writer.println("// Reference " + reference.getFromAddress() + " function " + function);
                    if (function != null) functions.add(function);
                }
            }
            DecompInterface decompiler = new DecompInterface();
            decompiler.openProgram(currentProgram);
            try {
                for (Function function : functions) {
                    monitor.checkCancelled();
                    writer.println("\n// Function " + function.getEntryPoint());
                    DecompileResults result = decompiler.decompileFunction(function, 30, monitor);
                    if (result.decompileCompleted()) writer.println(result.getDecompiledFunction().getC());
                    else writer.println("// Decompilation incomplete: " + result.getErrorMessage());
                }
            } finally { decompiler.dispose(); }
        }
    }
}
