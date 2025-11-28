using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BaseModel : DocumentList
    {
        protected virtual DocumentList CreateDemo() => new DocumentList();

        public BsonData.Collection Collection { get; set; }
        public BaseModel() { }
        public BaseModel(string name)
        {
            Collection = DB.Main.GetCollection(name);
        }

        bool _all_data_loaded;
        public BaseModel LoadData(bool reset = false)
        {
            if (reset || _all_data_loaded == false)
            {
                _all_data_loaded = true;

                this.Clear();
                AddRange(Collection.Select());
            }
            return this;
        }

        public virtual BaseModel Insert(Document doc)
        {
            if (Collection.Insert(doc))
            {
                base.Add(doc);
            }
            return this;
        }
        public virtual BaseModel Delete(Document doc)
        {
            Remove(doc);
            Collection.Delete(doc);

            return this;
        }
        public virtual BaseModel Update(Document doc)
        {
            Collection.Update(doc);
            return this;
        }
    }
}
