using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IFileParserService
    {
        Task<string> ExtractTextAsync(Stream fileStream, string fileName);
    }
}
