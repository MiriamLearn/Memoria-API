using Amazon.S3.Model;
using BL.InterfaceServices;
using DL;
using DL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.services
{
    public class ImageService : IImageService
    {
        private readonly IDataContext _dataContext;
        public ImageService(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task AddImage(Image image)
        {
            //await _dataContext.Images.AddAsync(image);
            //var album = await _dataContext.Albums.Where(a => a.Id == image.AlbumId).FirstOrDefaultAsync();
            //if (album != null)
            //    album.imageList.Add(image);
            //await _dataContext.SaveChangesAsync();
            var album = await _dataContext.Albums.Where(a => a.Id == image.AlbumId).FirstOrDefaultAsync();
            if (album == null)
            {
                throw new Exception("AlbumId does not exist.");
            }

            await _dataContext.Images.AddAsync(image);
            //album.imageList.Add(image);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<List<Image>> GetAllImages()
        {
            //return await _dataContext.Images.ToListAsync();
            return await _dataContext.Images
                .Where(i => !i.IsDeleted)
                .ToListAsync();
        }

        public async Task<Image> GetImageById(int id)
        {
            //var image = await _dataContext.Images.FirstOrDefaultAsync(i => i.Id == id);
            //return image ?? throw new KeyNotFoundException("Image not found");
            var image = await _dataContext.Images
                .FirstOrDefaultAsync(i => i.Id == id && !i.IsDeleted);

            return image ?? throw new KeyNotFoundException("Image not found");
        }
        public async Task UpdateImage(int id, Image image)
        {
            //var existingImage = await GetImageById(id);
            //if (existingImage == null)
            //    throw new KeyNotFoundException("Image not found");

            //// מסמן את התמונה הישנה כלא פעילה במקום למחוק
            //existingImage.IsDeleted = true;
            //existingImage.UpdatedAt = DateTime.UtcNow;

            //// יוצר תמונה חדשה עם המידע החדש
            //var updatedImage = new Image
            //{
            //    Name = newImage.Name,
            //    Type = newImage.Type,
            //    Size = newImage.Size,
            //    S3URL = newImage.S3URL,
            //    CreatedAt = DateTime.UtcNow
            //};

            //_dataContext.Images.Add(updatedImage);
            //await _dataContext.SaveChangesAsync();
            var imageT = await GetImageById(id);
            if (imageT == null)
                throw new KeyNotFoundException("Image not found");
            imageT.Name = image.Name;
            imageT.Type = image.Type;
            imageT.Size = image.Size;
            imageT.S3URL = image.S3URL;
            imageT.UpdatedAt = DateTime.UtcNow;
            await _dataContext.SaveChangesAsync();
        }

        public async Task RemoveImage(int id)
        {

            var imageToDelete = await GetImageById(id);
            if (imageToDelete == null)
                throw new KeyNotFoundException("Image not found");
            imageToDelete.IsDeleted = true;
            imageToDelete.UpdatedAt = DateTime.UtcNow;
            await _dataContext.SaveChangesAsync();

            //await UpdateImage(imageToDelete.Id, imageToDelete);
        }


    }
}
