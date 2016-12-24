using System;

namespace ASPWiki.Model
{
    public class SessionPath
    {
        public string id;
        public string path;
        public DateTime? created;

        public SessionPath(string id, string path)
        {
            this.id = id;
            this.path = path;
        }

        public SessionPath(string id, string path, DateTime? created)
        {
            this.id = id;
            this.path = path;
            this.created = created;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() + (path != null ? path.GetHashCode() : 0);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SessionPath);
        }
        public bool Equals(SessionPath obj)
        {
            return obj != null && obj.id == this.id && obj.path == this.path;
        }
    }
}
