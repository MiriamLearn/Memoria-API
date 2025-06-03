using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL.Entities
{
    public class CollageDesignResponse
    {
        public string Layout { get; set; } // "grid", "mosaic", "artistic", "magazine"
        public string Style { get; set; } // "modern", "vintage", "minimalist", "vibrant"
        public int ImageCount { get; set; }
        public int Spacing { get; set; }
        public int BorderRadius { get; set; }
        public bool AddText { get; set; }
        public string BackgroundType { get; set; } // תוכן הטקסט שיופיע בעיצוב
        public string Explanation { get; set; } // הסבר למשתמש מה ה-AI החליט
    }

    public class CollagePromptRequest
    {
        public string Prompt { get; set; }
    }
}
