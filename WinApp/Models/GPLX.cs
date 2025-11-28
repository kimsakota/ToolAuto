using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal class Saved : BaseModel
    {
        public Saved()
        {
            Collection = DB.Main.GetCollection("Saved");
        }
    }
    internal class GPLX : BaseModel
    {
        public BaseModel Saved
        {
            get
            {
                if (_saved == null)
                    _saved = new Saved();
                return _saved;
            }
        }
        BaseModel _saved;

        public GPLX() : base("gplx") { }
        public void Done()
        {
            if (Current != null)
            {
                Delete(Current);

                Current.Time = DateTime.Now;
                Saved.Insert(Current);

                Current = null;
            }
        }
        public int Import(DocumentList data)
        {
            int count = 0;
            foreach (Document doc in data) {
                if (Saved.Collection.Find(doc.ObjectId) != null)
                    continue;

                if (Collection.Insert(doc))
                {
                    this.Add(doc);
                    count++;
                }
            }
            return count;
        }
        public Document Current { get; set; }
    }
}
