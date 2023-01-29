using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;

        Random random = new Random();

        private int CELL_SIZE = Properties.Settings.Default.CELL_SIZE;

        ICollection<Cell> OldGeneration;
        ICollection<Cell> NewGeneration;

        readonly Brush DeadCellBrush = Brushes.White; // Цвет мертвых клетов  
        readonly Brush RedCellBrush = Brushes.Red; // Цвет красных клетов  
        readonly Brush GreenCellBrush = Brushes.Green; // Цвет зеленых клетов  

        public MainWindow()
        {
            InitializeComponent();

            InitializeTimer();

            InitializeBoard();
        }

        /// <summary>
        /// Создание таймера
        /// </summary>
        void InitializeTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Tick += delegate { UpdateBoard(); };
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
        }

        /// <summary>
        /// Разрисовка поля
        /// </summary>
        void InitializeBoard()
        {
            Board.Width = CELL_SIZE * Properties.Settings.Default.BOARD_ROWS_NUMBER;
            Board.Height = CELL_SIZE * Properties.Settings.Default.BOARD_COLUMNS_NUMBER;

            OldGeneration = new List<Cell>();
            NewGeneration = new List<Cell>();

            OldGenerationList();
            NewGenerationList();
        }

        /// <summary>
        /// Генерация списка для старого поколения
        /// </summary>
        private void OldGenerationList()
        {
            for (int currentRow = 0; currentRow < Properties.Settings.Default.BOARD_ROWS_NUMBER; currentRow++)
            {
                for (int currentColumn = 0; currentColumn < Properties.Settings.Default.BOARD_COLUMNS_NUMBER; currentColumn++)
                {
                    int x = currentRow * CELL_SIZE,
                        y = currentColumn * CELL_SIZE;

                    var newCell = new Cell(x, y, false);

                    OldGeneration.Add(newCell);

                    DrawingCells(newCell);
                }
            }
        }

        /// <summary>
        /// Генерация списка для нового поколения
        /// </summary>
        private void NewGenerationList()
        {
            NewGeneration.Clear();

            foreach (var item in OldGeneration)
            { 
                int x = item.X,
                    y = item.Y;

                var newCell = new Cell(x, y, item.IsAlive);

                NewGeneration.Add(newCell);
            }
        }

        /// <summary>
        /// Обновление поля для рисовки поля
        /// </summary>
        void UpdateBoard()
        {
            foreach (var cell in OldGeneration)
            {
                if (cell.FillCell == Brushes.Red)
                    DehaviorRedCells(cell);
                else if (cell.FillCell == Brushes.Green)
                    DehaviorDreenCells(cell);
                else
                    DehaviorDeadCells(cell);
            }

            OldGeneration.Clear();
            Board.Children.Clear();

            foreach (var cell in NewGeneration)
            {
                OldGeneration.Add(cell);

                UpdateBoardCell(cell);
            }

            foreach (var item in OldGeneration)
                DrawingCells(item);

            NewGenerationList();
        }

        /// <summary>
        /// Поведение клеток красного цвета 
        /// </summary>
        private void DehaviorRedCells(Cell cell)
        {
            var neighbourCellsNumber = GetCellNeighboursNumber(cell);

            if (neighbourCellsNumber.Count() == 4 && cell.IsAlive == false)
            {
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = true;
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).FillCell = ValidateFillCell(neighbourCellsNumber);
            }
            else if ((neighbourCellsNumber.Count() == 3 || neighbourCellsNumber.Count() == 4)
                && cell.IsAlive == true)
            {
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = true;
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).FillCell = ValidateFillCell(neighbourCellsNumber);
            }
            else
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = false;
        }

        /// <summary>
        /// Поведение мертвых клеток 
        /// </summary>
        private void DehaviorDreenCells(Cell cell)
        {
            var neighbourCellsNumber = GetCellNeighboursNumber(cell);

            if (neighbourCellsNumber.Count() == 3 && cell.IsAlive == false)
            {
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = true;
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).FillCell = ValidateFillCell(neighbourCellsNumber);
            }
            else if ((neighbourCellsNumber.Count() == 2 || neighbourCellsNumber.Count() == 3) && cell.IsAlive == true)
            {
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = true;
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).FillCell = ValidateFillCell(neighbourCellsNumber);
            }
            else
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = false;
        }

        /// <summary>
        /// Поведение клеток зеленых цвета 
        /// </summary>
        private void DehaviorDeadCells(Cell cell)
        {
            var neighbourCellsNumber = GetCellNeighboursNumber(cell);

            if (neighbourCellsNumber.Count() == 2 && cell.IsAlive == false)
            {
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = true;
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).FillCell = ValidateFillCell(neighbourCellsNumber);
            }
            else if ((neighbourCellsNumber.Count() == 2 || neighbourCellsNumber.Count() == 3) && cell.IsAlive == true)
            {
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = true;
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).FillCell = ValidateFillCell(neighbourCellsNumber);
            }
            else
                NewGeneration.First(newCell => newCell.X == cell.X && newCell.Y == cell.Y).IsAlive = false;
        }

        /// <summary>
        /// Проверка клетки на цвет
        /// </summary>
        private Brush ValidateFillCell(IEnumerable<Cell> ListNeighbourCells)
        {
            if (ListNeighbourCells.Count(c => c.FillCell == GreenCellBrush) > ListNeighbourCells.Count(c => c.FillCell == RedCellBrush))
                return GreenCellBrush;
            else if (ListNeighbourCells.Count(c => c.FillCell == GreenCellBrush) < ListNeighbourCells.Count(c => c.FillCell == RedCellBrush))
                return RedCellBrush;

            return random.Next(1, 2) % 2 == 0 ? GreenCellBrush : RedCellBrush;
        }

        /// <summary>
        /// Перебор всех клеток и передача всех соседних клеток
        /// </summary>
        private List<Cell> GetCellNeighboursNumber(Cell cell)
            => OldGeneration.Where(x =>
            (cell.X + CELL_SIZE == x.X && cell.Y == x.Y && x.IsAlive == true) 
            || (cell.X == x.X && cell.Y - CELL_SIZE == x.Y && x.IsAlive == true) 
            || (cell.X - CELL_SIZE == x.X && cell.Y == x.Y && x.IsAlive == true)
            || (cell.X == x.X && cell.Y + CELL_SIZE == x.Y && x.IsAlive == true)
            || (cell.X + CELL_SIZE == x.X && cell.Y + CELL_SIZE == x.Y && x.IsAlive == true) 
            || (cell.X - CELL_SIZE == x.X && cell.Y - CELL_SIZE == x.Y && x.IsAlive == true)
            || (cell.X + CELL_SIZE == x.X && cell.Y - CELL_SIZE == x.Y && x.IsAlive == true) 
            || (cell.X - CELL_SIZE == x.X && cell.Y + CELL_SIZE == x.Y && x.IsAlive == true))
            .ToList();

        /// <summary>
        /// Рисовка клеток на поле
        /// </summary>
        private void DrawingCells(Cell cell)
        {
            Board.Children.Add(cell.Rectangle);
            Canvas.SetLeft(cell.Rectangle, cell.X);
            Canvas.SetTop(cell.Rectangle, cell.Y);
        }

        /// <summary>
        /// Разрисовка клетки при нажатии на поле
        /// </summary>
        void MarkBoardCellAsLive(int mouseX, int mouseY, bool flag)
        {
            var cell = OldGeneration.FirstOrDefault(c =>
            c.X - CELL_SIZE <= mouseX
            && c.X + CELL_SIZE >= mouseX
            && c.Y - CELL_SIZE <= mouseY 
            && c.Y + CELL_SIZE >= mouseY);

            if (flag == true)
                cell.FillCell = Brushes.Green;
            else
                cell.FillCell = Brushes.Red;

            cell.IsAlive = true;

            UpdateBoardCell(cell);
            NewGenerationList();
        }

        /// <summary>
        /// Обновление цвета клетки исходя из свойста IsAlive
        /// </summary>
        void UpdateBoardCell(Cell cell)
            => cell.Rectangle.Fill = cell.IsAlive ? cell.FillCell : DeadCellBrush;

        /// <summary>
        /// Устанавливаем нажатие на мыши и определить координаты курсора при нажатии
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int mouseX = Convert.ToInt32(e.GetPosition(Board).X),
                   mouseY = Convert.ToInt32(e.GetPosition(Board).Y);

            if (Mouse.LeftButton == MouseButtonState.Pressed)
                MarkBoardCellAsLive(mouseX, mouseY, true);
            else
                MarkBoardCellAsLive(mouseX, mouseY, false);
        }

        /// <summary>
        /// Определение нажатие на клавиатуре и определение клавиши
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
                Timer.Stop();

            if (e.Key == Key.Space)
                Timer.Start();
        }
    }
}
