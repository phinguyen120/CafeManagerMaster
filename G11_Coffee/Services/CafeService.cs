using G11_Coffee.Models;

public class CafeService
{
    private readonly ApplicationDbContext _context;

    public CafeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Cafe> GetAllCafes()
    {
        return _context.Cafes.ToList();
    }

    public Cafe GetCafeById(int id)
    {
        return _context.Cafes.Find(id);
    }

    public void AddCafe(Cafe cafe)
    {
        _context.Cafes.Add(cafe);
        _context.SaveChanges();
    }

    public void UpdateCafe(Cafe cafe)
    {
        _context.Cafes.Update(cafe);
        _context.SaveChanges();
    }

    public void DeleteCafe(int id)
    {
        var cafe = _context.Cafes.Find(id);
        if (cafe != null)
        {
            _context.Cafes.Remove(cafe);
            _context.SaveChanges();
        }
    }
}
