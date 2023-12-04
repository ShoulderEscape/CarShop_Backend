using Entites;

namespace Data.Repositories
{
    public interface IArticleRepository
    {
        Task<Car> AddArticle(Car car);
        Task<Car> DeleteCarById(int id);
        Task<Car> GetArticle(int id);
        Task<List<Car>> GetArticle();
        Task<Car> UpdateCar(int id, Car car);
    }
}