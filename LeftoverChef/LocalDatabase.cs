// File: LocalDatabase.cs
// SQLite database manager
// Handles all data CRUD operations
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeftoverChef;

public class LocalDatabase
{
    // Database connection object
    private readonly SQLiteAsyncConnection _connection;

    public LocalDatabase(string dbPath)
    {
        // Connect to DB file
        _connection = new SQLiteAsyncConnection(dbPath);

        // Create tables if not exist
        _connection.CreateTableAsync<Recipe>().Wait();
        _connection.CreateTableAsync<Ingredient>().Wait();
    }

    
    //  Recipe operations
    
    public async Task<List<Recipe>> GetRecipesAsync()
    {
        return await _connection.Table<Recipe>().ToListAsync();
    }

    public async Task<int> SaveRecipeAsync(Recipe recipe)
    {
        if (recipe.Id != 0) return await _connection.UpdateAsync(recipe);
        else return await _connection.InsertAsync(recipe);
    }

    public async Task<int> DeleteRecipeAsync(Recipe recipe)
    {
        return await _connection.DeleteAsync(recipe);
    }

    
    //  Ingredient operations
    
    public async Task<List<Ingredient>> GetIngredientsAsync()
    {
        return await _connection.Table<Ingredient>().ToListAsync();
    }

    public async Task<int> SaveIngredientAsync(Ingredient item)
    {
        if (item.Id != 0) return await _connection.UpdateAsync(item);
        else return await _connection.InsertAsync(item);
    }

    public async Task<int> DeleteIngredientAsync(Ingredient item)
    {
        return await _connection.DeleteAsync(item);
    }
}