using Microsoft.EntityFrameworkCore;
using MvcMovieISA2.Domain;

namespace MvcMovieISA2.Infra;

public class Repo<T>(DbContext c) where T : Entity {
    private readonly DbContext db = c;
    private readonly DbSet<T> set = c.Set<T>();

    public async Task<List<T>> Get() => await set.ToListAsync();

    public async Task<T?> Get(int? id) => (id == null) ? null : await set.FindAsync(id);

    public async Task Delete(int? id){
        var movie = await Get(id);
        if (movie != null) set.Remove(movie);
        await db.SaveChangesAsync();
    }

    public async Task Update(T entity){
        db.Update(entity);
        await db.SaveChangesAsync();
    }

    public async Task Add(T entity){
        db.Add(entity);
        await db.SaveChangesAsync();
    }
}
