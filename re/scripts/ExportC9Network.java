// Export networking import references and selected decompilations without patching the executable.
// @category C9Research
import java.io.*;
import java.nio.charset.StandardCharsets;
import java.util.*;
import java.util.regex.Pattern;
import ghidra.app.script.GhidraScript;
import ghidra.app.decompiler.*;
import ghidra.program.model.address.*;
import ghidra.program.model.listing.*;
import ghidra.program.model.symbol.*;
import ghidra.program.model.mem.*;

public class ExportC9Network extends GhidraScript {
    private LinkedHashMap<Address, Function> selected = new LinkedHashMap<>();
    private PrintWriter refs;
    private String trigger;

    private void walk(Address target, int depth, Set<Address> visited) {
        if (depth > 2 || !visited.add(target)) return;
        for (Reference ref : getReferencesTo(target)) {
            Address from = ref.getFromAddress();
            Function f = getFunctionContaining(from);
            refs.println(trigger + "\t" + target + "\t" + from + "\t" +
                (f == null ? "" : f.getEntryPoint()) + "\t" + (f == null ? "" : f.getName()) + "\t" + ref.getReferenceType());
            if (f != null) selected.put(f.getEntryPoint(), f);
            else walk(from, depth + 1, visited);
        }
    }

    public void run() throws Exception {
        String[] args = getScriptArgs();
        File dir = new File(args[0]);
        dir.mkdirs();
        Pattern network = Pattern.compile("(?i)^(connect|send|recv|sendto|recvfrom|socket|closesocket|select|WSAAsyncSelect|WSAEventSelect|WSARecv|WSASend|WSAConnect|WSASocketA|WSAStartup|gethostbyname|inet_addr|htons|GetCommandLineA|GetCommandLineW|InternetConnectA|HttpSendRequestA)$");
        try (PrintWriter meta = new PrintWriter(new File(dir, "program.txt"), StandardCharsets.UTF_8)) {
            meta.println("Program: " + currentProgram.getName());
            meta.println("Executable SHA256: " + currentProgram.getExecutableSHA256());
            meta.println("Language: " + currentProgram.getLanguageID());
            meta.println("Image base: " + currentProgram.getImageBase());
            meta.println("Functions: " + currentProgram.getFunctionManager().getFunctionCount());
            for (MemoryBlock b : currentProgram.getMemory().getBlocks())
                meta.println("Block: " + b.getName() + " " + b.getStart() + "-" + b.getEnd());
        }
        refs = new PrintWriter(new File(dir, "network_xrefs.tsv"), StandardCharsets.UTF_8);
        refs.println("trigger\ttarget\treference_from\tfunction_entry\tfunction_name\treference_type");
        SymbolIterator syms = currentProgram.getSymbolTable().getAllSymbols(true);
        while (syms.hasNext()) {
            monitor.checkCancelled();
            Symbol sym = syms.next();
            if (network.matcher(sym.getName()).matches() ||
                sym.getName().contains("@Network@BaseCat@@") ||
                sym.getName(true).contains("BaseCat::Network::") ||
                (sym.getParentNamespace().getName().equalsIgnoreCase("WS2_32.dll") &&
                 sym.getName().matches("Ordinal_(3|4|9|10|12|16|17|18|19|20|23|52|115|116)"))) {
                trigger = sym.getName();
                walk(sym.getAddress(), 0, new HashSet<>());
            }
        }
        // Follow string references at verified VAs supplied by the inventory review.
        if (args.length > 1) {
            try (BufferedReader in = new BufferedReader(new FileReader(args[1], StandardCharsets.UTF_8))) {
                String line;
                while ((line = in.readLine()) != null) {
                    String[] cols = line.split("\t", 2);
                    if (cols.length != 2) continue;
                    trigger = "string:" + cols[1];
                    Address a = toAddr(cols[0]);
                    if (a != null && cols[1].startsWith("code:")) {
                        Function f = getFunctionContaining(a);
                        refs.println(trigger + "\t" + a + "\t" + a + "\t" +
                            (f == null ? "" : f.getEntryPoint()) + "\t" +
                            (f == null ? "" : f.getName()) + "\tCODE_SEED");
                        if (f != null) {
                            LinkedHashMap<Address, Function> priority = new LinkedHashMap<>();
                            priority.put(f.getEntryPoint(), f);
                            priority.putAll(selected);
                            selected = priority;
                        }
                    }
                    else if (a != null) walk(a, 0, new HashSet<>());
                }
            }
        }
        refs.close();
        // Include direct callers to expose setup and dispatch around socket wrappers.
        List<Function> seeds = new ArrayList<>(selected.values());
        for (Function seed : seeds) {
            for (Reference ref : getReferencesTo(seed.getEntryPoint())) {
                Function caller = getFunctionContaining(ref.getFromAddress());
                if (caller != null && selected.size() < 90) selected.put(caller.getEntryPoint(), caller);
            }
        }
        DecompInterface decomp = new DecompInterface();
        decomp.openProgram(currentProgram);
        try (PrintWriter out = new PrintWriter(new File(dir, "network_decompiled.c"), StandardCharsets.UTF_8)) {
            int count = 0;
            for (Function f : selected.values()) {
                monitor.checkCancelled();
                if (count++ >= 90) break;
                out.println("\n/* " + f.getEntryPoint() + " " + f.getName() + " */");
                DecompileResults r = decomp.decompileFunction(f, 15, monitor);
                if (r.decompileCompleted()) out.println(r.getDecompiledFunction().getC());
                else out.println("/* Decompile unavailable: " + r.getErrorMessage() + " */");
            }
        } finally { decomp.dispose(); }
        println("C9_NETWORK_EXPORT_COMPLETE functions=" + selected.size() + " output=" + dir);
    }
}
