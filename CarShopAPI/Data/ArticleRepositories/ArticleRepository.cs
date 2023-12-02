using CarShopAPI.Data;
using Entites;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ArticleRepository : IArticleRepository
{
    private readonly DataContext _context;

    public ArticleRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Car> AddArticle(Car car)
    {
        _context.Cars.Add(car);
        await _context.SaveChangesAsync();

        return car;
    }

    public async Task<Car> DeleteCarById(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null)
        {
            return null;
        }
        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return car;
    }

    public async Task<Car> GetArticle(int id)
    {
        return await _context.Cars.FindAsync(id);
    }

    public Task<List<Car>> GetArticle()
    {
        return _context.Cars.ToListAsync();
    }

    public async Task<Car> UpdateCar(int id, Car car)
    {
        if (id != car.Id)
        {
            return null;
        }
        _context.Entry(car).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return car;
    }
}
