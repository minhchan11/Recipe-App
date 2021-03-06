using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Xunit;


namespace RecipeApp
{
  public class RecipeTest: IDisposable
  {
    public RecipeTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=recipe_test;Integrated Security=SSPI;";
    }

    public void Dispose()
    {
      Category.DeleteAll();
      Recipe.DeleteAll();
      Ingredient.DeleteAll();
    }

    [Fact]
    public void GetAll_DatabaseEmptyAtFirst_ZeroOutput()
    {
      //Arrange, Act
      int result = Recipe.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }

    [Fact]
    public void OverrideBool_SameRecipe_ReturnsEqual()
    {
      //Arrange, Act
      Recipe recipeOne = new Recipe ("Pot Pie", "Microwave it");
      Recipe recipeTwo = new Recipe ("Pot Pie", "Microwave it");

      //Assert
      Assert.Equal(recipeTwo, recipeOne);
    }

    [Fact]
    public void Save_OneRecipe_RecipeSavedToDatabase()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");

      //Act
      testRecipe.Save();
      List<Recipe> output = Recipe.GetAll();
      List<Recipe> verify = new List<Recipe>{testRecipe};

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void Save_OneRecipe_RecipeSavedWithCorrectID()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();
      Recipe savedRecipe = Recipe.GetAll()[0];

      //Act
      int output = savedRecipe.GetId();
      int verify = testRecipe.GetId();

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void SaveGetAll_ManyRecipes_ReturnListOfRecipes()
    {
      //Arrange
      Recipe recipeOne = new Recipe ("Pot Pie", "Microwave it");
      recipeOne.Save();
      Recipe recipeTwo = new Recipe ("Instant Ramen", "Boil it");
      recipeTwo.Save();

      //Act
      List<Recipe> output = Recipe.GetAll();
      List<Recipe> verify = new List<Recipe>{recipeOne, recipeTwo};

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void Find_OneRecipeId_ReturnRecipeFromDatabase()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();

      //Act
      Recipe foundRecipe = Recipe.Find(testRecipe.GetId());

      //Assert
      Assert.Equal(testRecipe, foundRecipe);
    }

    [Fact]
    public void SearchName_Name_ReturnRecipeFromDatabase()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();

      //Act
      List<Recipe> output = Recipe.SearchName("Pot Pie");
      List<Recipe> verify = new List<Recipe>{testRecipe};

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void AddCategory_OneRecipe_CategoryAddedToJoinTable()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();
      Category testCategory = new Category("Peasant");
      testCategory.Save();
      testRecipe.AddCategory(testCategory);

      //Act
      List<Category> output = testRecipe.GetCategory();
      List<Category> verify = new List<Category>{testCategory};

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void AddIngredient_OneRecipe_IngredientAddedToJoinTable()
    {
      //Arrange
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();
      Ingredient testIngredient = new Ingredient("Pepper");
      testIngredient.Save();
      testRecipe.AddIngredient(testIngredient, "1 dash ");

      //Act
      List<Ingredient> outputList = testRecipe.GetIngredient();
      List<Ingredient> verifyList = new List<Ingredient>{testIngredient};

      string output = outputList[0].GetAmount() + outputList[0].GetName() ;
      string verify = "1 dash Pepper";

      //Assert
      Assert.Equal(verify, output);
    }

    [Fact]
    public void Recipe_Delete_RemoveObjectFromDatabase()
    {
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();

      testRecipe.Delete();

      Assert.Equal(0, Recipe.GetAll().Count);
    }

    [Fact]
    public void Recipe_Update_UpdateDatabaseAndLocalObject()
    {
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();

      testRecipe.Update("Chicken Pot Pie", "Boil it", 4);
      Recipe expectedRecipe = new Recipe("Chicken Pot Pie", "Boil it", 4, testRecipe.GetId());

      Assert.Equal(expectedRecipe, Recipe.Find(testRecipe.GetId()));
    }

    [Fact]
    public void Recipe_UpdateAmount_UpdateJoinTabaleAndLocalObject()
    {
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();
      Ingredient testIngredient = new Ingredient("Pepper");
      testIngredient.Save();
      testRecipe.AddIngredient(testIngredient, "1 dash ");

      testRecipe.UpdateAmount(testIngredient, "1 pound ");
      List<Ingredient> outputList = testRecipe.GetIngredient();
      List<Ingredient> verifyList = new List<Ingredient>{testIngredient};

      string output = outputList[0].GetAmount() + outputList[0].GetName() ;
      string verify = "1 pound Pepper";

      Assert.Equal(verify, output);
    }

    [Fact]
    public void Recipe_Save_NoSaveOnDuplicateRecipe()
    {
      Recipe testRecipe = new Recipe ("Pot Pie", "Microwave it");
      testRecipe.Save();
      Recipe secondRecipe = new Recipe ("Pot Pie", "Microwave it");
      secondRecipe.Save();

      Assert.Equal(1, Recipe.GetAll().Count);
    }
















  }
}
