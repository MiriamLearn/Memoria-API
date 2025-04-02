using BL.InterfaceServices;
using DL;
using DL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.services
{
    public class AlbumService : IAlbumService
    {
        private readonly IDataContext _dataContext;
        public AlbumService(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        //public async Task<List<Album>> GetUserAlbums(int userId)
        //{
        //    //return await _dataContext.Albums
        //    //                          .Where(a => a.User.Id == userId && !a.IsDeleted)
        //    //                          .ToListAsync();
        //    return await _dataContext.Albums
        //                    .Where(a => a.User != null && a.User.Id == userId && !a.IsDeleted)
        //                    .ToListAsync();
        //}

        public async Task<Album> AddAlbum(Album album)
        {
           await _dataContext.Albums.AddAsync(album);
            await _dataContext.SaveChangesAsync();
            return album;
        }


        public async Task<Album> GetAlbumById(int id)
        {
            //var album = await _dataContext.Albums.Include(a => a.imageList).FirstOrDefaultAsync(a => a.Id == id);
            //return album ?? throw new KeyNotFoundException("Album not found");
            var album = await _dataContext.Albums
                                  .Include(a => a.imageList)
                                  .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
            return album ?? throw new KeyNotFoundException("Album not found");
        }

        public async Task<List<Album>> GetAllAlbums()
        {
            //return await _dataContext.Albums.ToListAsync();
            return await _dataContext.Albums
                            .Where(a => !a.IsDeleted)
                            .ToListAsync();
        }
        public async Task UpdateAlbum(int id, Album album)
        {
            var Album = await GetAlbumById(id);
            if (Album == null)
                throw new KeyNotFoundException("Album not found");
            Album.Name = album.Name;
            Album.UpdatedAt = DateTime.Now;
            await _dataContext.SaveChangesAsync();
        }

        public async Task RemoveAlbum(int id)
        {
            //var albumToDelete = await GetAlbumById(id);
            //if (albumToDelete == null)
            //    throw new KeyNotFoundException("Album not found");
            //albumToDelete.IsDeleted = true;
            //await _dataContext.SaveChangesAsync();
            var albumToDelete = await GetAlbumById(id);
            if (albumToDelete == null)
                throw new KeyNotFoundException("Album not found");

            if (albumToDelete.imageList.Count > 0)
                throw new InvalidOperationException("Cannot delete album with images");

            albumToDelete.IsDeleted = true;
            await _dataContext.SaveChangesAsync();
        }

      
    }
}
