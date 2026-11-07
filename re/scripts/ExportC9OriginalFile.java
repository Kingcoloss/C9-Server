// Recover only the original imported file bytes from a research database.
// This does not read a running process or unpack/modify protected code.
// @category C9Research
import ghidra.app.script.GhidraScript;
import ghidra.program.database.mem.FileBytes;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.security.MessageDigest;
import java.util.HexFormat;

public class ExportC9OriginalFile extends GhidraScript {
    public void run() throws Exception {
        String[] args = getScriptArgs();
        if (args.length != 2 || !args[1].matches("[0-9a-f]{64}"))
            throw new IllegalArgumentException("Output path and expected original SHA-256 required");
        Path output = Path.of(args[0]).toAbsolutePath().normalize();
        if (!output.toString().replace('\\', '/').contains("/artifacts/"))
            throw new IllegalArgumentException("Keep recovered Client bytes in ignored artifacts");
        for (FileBytes source : currentProgram.getMemory().getAllFileBytes()) {
            if (!source.getFilename().equalsIgnoreCase("C9.exe")) continue;
            if (source.getSize() <= 0 || source.getSize() > 64 * 1024 * 1024)
                throw new IllegalArgumentException("Imported file exceeds bounded recovery size");
            byte[] data = new byte[(int)source.getSize()];
            if (source.getOriginalBytes(0, data) != data.length)
                throw new IllegalStateException("Incomplete original file");
            String digest = HexFormat.of().formatHex(MessageDigest.getInstance("SHA-256").digest(data));
            if (!digest.equals(args[1])) throw new IllegalStateException("Original import hash mismatch");
            Files.createDirectories(output.getParent());
            if (Files.exists(output)) {
                if (!MessageDigest.isEqual(Files.readAllBytes(output), data))
                    throw new IllegalStateException("Existing output differs; leaving it untouched");
            } else Files.write(output, data, StandardOpenOption.CREATE_NEW);
            println("C9_ORIGINAL_IMPORT_VERIFIED " + digest + " bytes=" + data.length);
            return;
        }
        throw new IllegalStateException("Original C9.exe file bytes not present in this database");
    }
}
