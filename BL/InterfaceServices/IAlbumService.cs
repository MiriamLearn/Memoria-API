using DL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.InterfaceServices
{
    public interface IAlbumService
    {
        public Task<List<Album>> GetAllAlbums();
        public Task<Album> GetAlbumById(int id);
        //public Task<List<Album>> GetUserAlbums(int userId);
        public Task<Album> AddAlbum(Album album);
        public Task UpdateAlbum(int id, Album album);
        public Task RemoveAlbum(int id);
    }
}
