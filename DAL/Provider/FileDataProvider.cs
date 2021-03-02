using DAL.Entity;
using DAL.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Provider
{
    public class FileDataProvider : DataProvider<File>, IFileDataProvider
    {
        private readonly Context Context;

        public FileDataProvider(Context context) : base(context)
        {
            Context = context;
        }

        public File Get(Guid id)
        {
            return Get(f => f.Id.Equals(id));
        }

        public new File Add(File file)
        {
            base.Add(file);
            return file;
        }
    }
}
