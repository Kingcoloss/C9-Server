using System.Globalization;
using System.Text;
using C9.Contracts;
using C9.Server;

namespace C9.Portal.Services;

/// <summary>Generates a Wireshark Lua dissector for the "decoded" pcap export (UDP port 49099).</summary>
public static class WiresharkDissector
{
    public static string Lua()
    {
        StringBuilder names = new();
        foreach (PacketTypeInfo packet in KnownPackets.All)
            names.Append(CultureInfo.InvariantCulture, $"  [0x{packet.Type:X4}] = \"{packet.Name}\",\n");
        return $$"""
            -- C9 decoded frames exported by the C9 Admin Portal ("pcap (decoded)").
            -- Install: copy to the Wireshark personal plugins folder (Help > About > Folders), then reload Lua (Ctrl+Shift+L).
            -- Each UDP datagram on port {{PcapExport.DecodedPort}} holds one decoded frame: u16 length, u8 flag, u16 type,
            -- u32 checksum, u32 sequence, payload. Length 0xFFFF marks a frame that failed to decode or was redacted.
            local names = {
            {{names}}}
            local c9 = Proto("c9", "C9 decoded frame")
            local f_length = ProtoField.uint16("c9.length", "Length", base.DEC)
            local f_flag = ProtoField.uint8("c9.flag", "Flag", base.DEC, { [0] = "plain", [1] = "encrypted" })
            local f_type = ProtoField.uint16("c9.type", "Type", base.HEX, names)
            local f_checksum = ProtoField.uint32("c9.checksum", "Checksum", base.HEX)
            local f_sequence = ProtoField.uint32("c9.sequence", "Sequence", base.DEC)
            local f_payload = ProtoField.bytes("c9.payload", "Payload")
            local f_direction = ProtoField.string("c9.direction", "Direction")
            c9.fields = { f_length, f_flag, f_type, f_checksum, f_sequence, f_payload, f_direction }

            function c9.dissector(buffer, pinfo, tree)
              if buffer:len() < 13 then return 0 end
              pinfo.cols.protocol = "C9"
              local direction = (pinfo.src_port == {{PcapExport.DecodedPort}}) and "s2c" or "c2s"
              local subtree = tree:add(c9, buffer(), "C9 frame")
              subtree:add(f_direction, direction)
              local length = buffer(0, 2):le_uint()
              local item = subtree:add_le(f_length, buffer(0, 2))
              if length == 0xFFFF then item:append_text(" (not decoded or redacted)") end
              subtree:add_le(f_flag, buffer(2, 1))
              subtree:add_le(f_type, buffer(3, 2))
              subtree:add_le(f_checksum, buffer(5, 4))
              subtree:add_le(f_sequence, buffer(9, 4))
              if buffer:len() > 13 then subtree:add(f_payload, buffer(13)) end
              local packet_type = buffer(3, 2):le_uint()
              pinfo.cols.info = string.format("%s 0x%04X %s payload=%d", direction, packet_type, names[packet_type] or "", buffer:len() - 13)
              return buffer:len()
            end

            DissectorTable.get("udp.port"):add({{PcapExport.DecodedPort}}, c9)
            """;
    }
}
