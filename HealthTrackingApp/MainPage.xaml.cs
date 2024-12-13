using Microsoft.Maui.Controls;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HealthTrackingApp
{
    public partial class MainPage : ContentPage
    {
        private SQLiteConnection _db;
        private string _dbPath => Path.Combine(FileSystem.AppDataDirectory, "healthtracking.db");

        public MainPage()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadStatistics();
        }

        private void InitializeDatabase()
        {
            _db = new SQLiteConnection(_dbPath);
            _db.CreateTable<ExerciseData>();
            _db.CreateTable<FoodData>();
            _db.CreateTable<SleepData>();
        }

        private void LoadStatistics()
        {
            // Загрузка статистики
            int totalSteps = _db.Table<ExerciseData>().Sum(e => e.Steps);
            int totalCalories = _db.Table<FoodData>().Sum(f => f.Calories);
            int totalSleep = _db.Table<SleepData>().Sum(s => s.Hours);

            // Обновление UI
            StepsLabel.Text = $"Шаги: {totalSteps}";
            CaloriesLabel.Text = $"Калории: {totalCalories}";
            SleepLabel.Text = $"Сон: {totalSleep} ч";
        }

        private async void OnAddDataClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Выберите тип данных", "Отмена", null, "Упражнения", "Питание", "Сон");
            if (action == "Упражнения")
            {
                string steps = await DisplayPromptAsync("Упражнения", "Введите количество шагов:");
                _db.Insert(new ExerciseData { Steps = int.Parse(steps) });
            }
            else if (action == "Питание")
            {
                string calories = await DisplayPromptAsync("Питание", "Введите количество калорий:");
                _db.Insert(new FoodData { Calories = int.Parse(calories) });
            }
            else if (action == "Сон")
            {
                string hours = await DisplayPromptAsync("Сон", "Введите количество часов сна:");
                _db.Insert(new SleepData { Hours = int.Parse(hours) });
            }
            LoadStatistics();
        }

        private void OnShowStatisticsClicked(object sender, EventArgs e)
        {
            // Пример отображения статистики
            var exerciseData = _db.Table<ExerciseData>().ToList();
            // Используйте библиотеку графиков для отображения данных
        }
    }

    // Модели для базы данных
    public class ExerciseData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Steps { get; set; }
    }

    public class FoodData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Calories { get; set; }
    }

    public class SleepData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int Hours { get; set; }
    }
}
