"""List CFS archive entries using the project's bounded reader (index only).

python cfs_list.py <out.json> <archive.cfs>...
"""
import json
import sys

sys.path.insert(0, r"C:\MyProjects\C9-Server\re\scripts")
from c9_cfs import Archive  # noqa: E402

out = {}
for p in sys.argv[2:]:
    try:
        a = Archive(p)
        out[p] = {n: e.size for n, e in a.entries.items()}
        print(p, len(a.entries))
    except Exception as ex:  # noqa
        out[p] = {"error": str(ex)}
        print(p, "ERR", ex)
json.dump(out, open(sys.argv[1], "w"), indent=0)
