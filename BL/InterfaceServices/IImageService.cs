using DL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.InterfaceServices
{
    public interface IImageService
    {
        public Task<List<Image>> GetAllImages();
        public Task<Image> GetImageById(int id);
        public Task AddImage(Image image);
        public Task UpdateImage(int id, Image image);
        public Task RemoveImage(int id);
    }
}
