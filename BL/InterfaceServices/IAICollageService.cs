using DL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.InterfaceServices
{
    public interface IAICollageService
    {
        Task<CollageDesignResponse> GenerateCollageDesignAsync(string userPrompt);
    }
}
