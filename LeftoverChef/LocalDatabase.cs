using SQLite;

namespace LeftoverChef;

public class LocalDatabase
{
    // 定义一个数据库连接
    private readonly SQLiteAsyncConnection _database;

    // 构造函数：告诉系统数据库建在哪里，并建表
    public LocalDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);

        // 自动根据你的模型创建这两张表（如果表已经存在，它什么都不做）
        _database.CreateTableAsync<Recipe>().Wait();
        _database.CreateTableAsync<Ingredient>().Wait();
    }

    // ==========================================
    // Recipe (菜谱) 的数据库操作
    // ==========================================

    // 获取所有菜谱
    public Task<List<Recipe>> GetRecipesAsync()
    {
        return _database.Table<Recipe>().ToListAsync();
    }

    // 把新菜谱存进数据库
    public Task<int> SaveRecipeAsync(Recipe recipe)
    {
        return _database.InsertAsync(recipe);
    }

    // 从数据库里彻底删掉某道菜
    public Task<int> DeleteRecipeAsync(Recipe recipe)
    {
        return _database.DeleteAsync(recipe);
    }


    // ==========================================
    // Ingredient (食材) 的数据库操作
    // ==========================================

    // 获取所有冰箱食材
    public Task<List<Ingredient>> GetIngredientsAsync()
    {
        return _database.Table<Ingredient>().ToListAsync();
    }

    // 把买来的新菜存进数据库
    public Task<int> SaveIngredientAsync(Ingredient ingredient)
    {
        return _database.InsertAsync(ingredient);
    }

    // 从数据库里删掉吃完的食材
    public Task<int> DeleteIngredientAsync(Ingredient ingredient)
    {
        return _database.DeleteAsync(ingredient);
    }
}