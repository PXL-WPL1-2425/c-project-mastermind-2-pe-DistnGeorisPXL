using Microsoft.VisualBasic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
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

namespace mastermind
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        string[,] highScores = new string[15, 3]; // 4 rijen en 2 kolommen

        bool codeCracked = false;


        int attempts = 0;


        int playerScore = 100;


        string name;

        string maxtrys = "10";

        private bool _isDebugMode = false;





        // Beschikbare kleuren
        List<string> kleuren = new List<string> { "Rood", "Geel", "Oranje", "Wit", "Groen", "Blauw" };



        //pogingen tellen van de speler
        private List<string> playerAttempts = new List<string>();





        // Lijst voor de gegenereerde code
        List<string> secretCode = new List<string>();






        private DispatcherTimer _countdownTimer;
        private int _currentTime;
        public MainWindow()
        {
            InitializeComponent();
            VulComboBoxen(); // Vul de comboboxen met de kleuren
            GenerateRandomCode(); // Genereer en toon de random code in de titel
            StartGame();
            // Voeg een globale toetsencombinatie toe voor Ctrl+F12
            this.KeyDown += MainWindow_KeyDown;






            // Timer instellen
            _countdownTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1) // Timer loopt elke seconde
            };
            _countdownTimer.Tick += CountdownTimer_Tick;





            // Zet de timer initieel op 0
            _currentTime = 0;
            UpdateCountdownLabel();
            StartCountdown();
        }

        private void StartGame()
        {
            string antwoord = Interaction.InputBox("Geef je naam", "naam", "50", 500);
            while (string.IsNullOrEmpty(antwoord))
            {
                MessageBox.Show("Geef je naam!", "Foutieve invoer");
                antwoord = Interaction.InputBox("Geef je naam", "Invoer", "50", 500);
            }
            name = antwoord.ToLower();
        }

        private void StartAgain()
        {

            //highScores.Cast(name, attempts, playerScore);
            if(attempts == 10)
            {
                MessageBox.Show($"You failed! De corecte code was " + string.Join(", ", secretCode) + ".\nNog eens proberen?", "FAILED", MessageBoxButton.OK, MessageBoxImage.Question);
            }
            if (codeCracked == true)
            {
                MessageBoxResult endGame = MessageBox.Show($"Code gekraakt in {attempts} pogingen.\nNog eens proberen?", "WON", MessageBoxButton.OK, MessageBoxImage.Question);
            }

        }






        /// <summary>
        /// Start de countdown timer vanaf 1 seconde en reset de huidige tijd.
        /// Wordt gebruikt bij het genereren van een nieuwe code of bij een poging.
        /// </summary>
        private void StartCountdown()
        {
            // Reset de timer en start opnieuw vanaf 1
            _currentTime = 1;
            UpdateCountdownLabel();




            if (_countdownTimer.IsEnabled)
            {
                _countdownTimer.Stop();
            }

            _countdownTimer.Start();
        }











        /// <summary>
        /// Stopt de countdown timer.
        /// Wordt gebruikt als een maximale tijdslimiet of poging is bereikt.
        /// </summary>
        private void StopCountdown()
        {
            _countdownTimer.Stop(); // Timer stoppen
            UpdateCountdownLabel();
        }






        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            // Verhoog de timer elke seconde
            _currentTime++;
            UpdateCountdownLabel();
            if (attempts >= 10)
            {
                StopCountdown();
                timerLabel.Content = "EINDE SPEL!";
            }

                // Controleer of de timer op 10 staat
                if (_currentTime == 10)
            {
                HandleTimerAtTen();
            }
        }












        private void HandleTimerAtTen()
        {
            // Actie uitvoeren als de timer op 10 komt
            _countdownTimer.Stop(); // Timer stoppen als voorbeeld
            attempts++;
            this.Title = $"Mastermind - Poging: {attempts}/10";
            if (attempts >= 10)
            {
                StopCountdown(); // Timer stoppen als voorbeeld
                timerLabel.Content = "EINDE SPEL!";
                StartAgain();
            }
            else
            {
                StartCountdown();
            }

        }











        private void UpdateCountdownLabel()
        {
            if(attempts == 10)
            {
                timerLabel.Content = "EINDE SPEL!";
            }
            else
            {
                timerLabel.Content = $"Tijd voor kans voorbij gaat: {_currentTime}/10 seconden";
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Controleer of de gebruiker CTRL+F12 indrukt
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.F12)
                {
                    ToggleDebug();
                }
            }
        }













        /// <summary>
        /// Schakelt de debug-modus in of uit.
        /// Wanneer de debug-modus actief is, wordt de geheime code getoond in een TextBox.
        /// </summary>
        private void ToggleDebug()
        {
            // Wissel debug-modus aan/uit
            _isDebugMode = !_isDebugMode;



            // Update de zichtbaarheid van de TextBox
            secretCodeTextBox.Visibility = _isDebugMode ? Visibility.Visible : Visibility.Collapsed;



            // Voeg debuginformatie toe als voorbeeld
            if (_isDebugMode)
            {
                secretCodeTextBox.Text = "Mastermind oplossing: " + string.Join(", ", secretCode);
            }
        }













        // Methode om de willekeurige code te genereren
        private void GenerateRandomCode()
        {


            // Random object voor willekeurige getallen
            Random random = new Random();



            // Genereer een willekeurige code van 4 kleuren
            for (int i = 0; i < 4; i++)
            {
                secretCode.Add(kleuren[random.Next(kleuren.Count)]);
            }




            // Zet de pogingen in de titel van het window
            this.Title = $"Mastermind - poging {attempts}/10";
        }









        private void VulComboBoxen()
        {
            comboBox1.ItemsSource = kleuren;
            comboBox2.ItemsSource = kleuren;
            comboBox3.ItemsSource = kleuren;
            comboBox4.ItemsSource = kleuren;
        }






        private void CheckComboBoxChanges(object sender)
        {
            // Controleer welke ComboBox is geselecteerd en werk het juiste Label bij
            if (sender == comboBox1)
            {
                UpdateLabelColor(label1, comboBox1.SelectedItem.ToString());
            }
            else if (sender == comboBox2)
            {
                UpdateLabelColor(label2, comboBox2.SelectedItem.ToString());
            }
            else if (sender == comboBox3)
            {
                UpdateLabelColor(label3, comboBox3.SelectedItem.ToString());
            }
            else if (sender == comboBox4)
            {
                UpdateLabelColor(label4, comboBox4.SelectedItem.ToString());
            }
        }




        // Event handler voor de selectie van een kleur in de ComboBox
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Controleer welke ComboBox is geselecteerd en werk het juiste Label bij
            CheckComboBoxChanges(sender);
        }











        // Methode om de achtergrondkleur van een label bij te werken
        private void UpdateLabelColor(System.Windows.Controls.Label label, string colorName)
        {
            // Zet de achtergrondkleur van het label op basis van de geselecteerde kleur
            switch (colorName)
            {
                case "Rood":
                    label.Background = System.Windows.Media.Brushes.Red;
                    break;
                case "Geel":
                    label.Background = System.Windows.Media.Brushes.Yellow;
                    break;
                case "Oranje":
                    label.Background = System.Windows.Media.Brushes.Orange;
                    break;
                case "Wit":
                    label.Background = System.Windows.Media.Brushes.White;
                    break;
                case "Groen":
                    label.Background = System.Windows.Media.Brushes.Green;
                    break;
                case "Blauw":
                    label.Background = System.Windows.Media.Brushes.Blue;
                    break;
                default:
                    label.Background = System.Windows.Media.Brushes.Transparent; // Als er geen kleur is geselecteerd
                    break;
            }
        }











        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);

            // Example: Show confirmation dialog
            var result = MessageBox.Show("Weet je zeker dat je het spel wilt sluiten?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                // Cancel the closing
                e.Cancel = true;
            }
            else
            {
                // Perform cleanup or save data
                return;
            }
        }









        // Event handler voor het klikken op de Check Code knop
        private void CheckButton_Click(object sender, RoutedEventArgs e)
        {
            if(attempts < 10)
            {




                // Haal de ingevoerde kleuren uit de ComboBoxen
                List<string> userInput = new List<string>
            {
                comboBox1.SelectedItem?.ToString(),
                comboBox2.SelectedItem?.ToString(),
                comboBox3.SelectedItem?.ToString(),
                comboBox4.SelectedItem?.ToString()
            };





            // Controleer of de gebruiker een waarde heeft geselecteerd in elke ComboBox
            if (userInput.Contains(null))
            {
                MessageBox.Show("Selecteer een kleur voor alle vakken!");
                return;
            }
                else
                {
                    StartCountdown();
                }



                // Voeg poging toe aan de lijst
                string attemptString = string.Join(", ", userInput);
                playerAttempts.Add(attemptString);



                // Update de ListBox
                attemptsListBox.Items.Add($"Poging {attempts + 1}: {attemptString}");




                //voeg 1 poging toe aan de attempts
                attempts++;


            //weergeef het geupdaten attempt variabel
            this.Title = $"Mastermind - Poging: {attempts}/10";






            // Vergelijk de ingevoerde code met de geheime code
            for (int i = 0; i < 4; i++)
            {

                // Als de kleur op de juiste plaats staat (Rood)
                if (userInput[i] == secretCode[i])
                {
                    SetLabelBorder(i, Colors.DarkRed); // Rode rand voor correcte positie
                }
                else if (secretCode.Contains(userInput[i]))
                {
                    SetLabelBorder(i, Colors.Wheat); // Witte rand voor correcte kleur maar verkeerde positie
                        playerScore--;
                        playerScoreTextBox.Text = $"Score: {playerScore}/100";
                    }
                else
                {
                    SetLabelBorder(i, Colors.Transparent); // Geen rand als de kleur niet in de code zit
                        playerScore -= 2;
                        playerScoreTextBox.Text = $"Score: {playerScore}/100";
                    }
            }

                if (attempts == 10)
                {
                    StartAgain();
                    StopCountdown();
                    _countdownTimer.Stop();
                }
                if (userInput.SequenceEqual(secretCode))
                {
                    codeCracked = true;
                    StartAgain();
                }
            }
            else
            {
                StopCountdown();
                MessageBox.Show("Het spel is al beeindigd, U heeft maximum aantal kansen gebruikt.");
            }
        }












        // Methode om de rand van het label in te stellen
        private void SetLabelBorder(int index, Color borderColor)
        {
            switch (index)
            {
                case 0:
                    label1.BorderBrush = new SolidColorBrush(borderColor);
                    label1.BorderThickness = new Thickness(2);
                    break;
                case 1:
                    label2.BorderBrush = new SolidColorBrush(borderColor);
                    label2.BorderThickness = new Thickness(2);
                    break;
                case 2:
                    label3.BorderBrush = new SolidColorBrush(borderColor);
                    label3.BorderThickness = new Thickness(2);
                    break;
                case 3:
                    label4.BorderBrush = new SolidColorBrush(borderColor);
                    label4.BorderThickness = new Thickness(2);
                    break;
            }
        }





        private void newGameMenu_Click(object sender, RoutedEventArgs e)
        {
            StartAgain2();

        }


        private void StartAgain2()
        {
            if (attempts == 10)
            {
                MessageBoxResult endGame = MessageBox.Show($"Weet je zeker dat je opnieuw wilt beginnen?", "Opnieuw beginnen", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (endGame == MessageBoxResult.Yes)
                {
                    secretCode.Clear();
                    label1.Background = System.Windows.Media.Brushes.White;
                    label1.BorderBrush = System.Windows.Media.Brushes.White;
                    label2.Background = System.Windows.Media.Brushes.White;
                    label2.BorderBrush = System.Windows.Media.Brushes.White;
                    label3.Background = System.Windows.Media.Brushes.White;
                    label3.BorderBrush = System.Windows.Media.Brushes.White;
                    label4.Background = System.Windows.Media.Brushes.White;
                    label4.BorderBrush = System.Windows.Media.Brushes.White;
                    comboBox1.SelectedItem = -1;
                    comboBox2.SelectedItem = -1;
                    comboBox3.SelectedItem = -1;
                    comboBox4.SelectedItem = -1;
                    attemptsListBox.Items.Clear();
                    GenerateRandomCode();
                    secretCodeTextBox.Text = "Mastermind oplossing: " + string.Join(", ", secretCode);
                    attempts = 0;
                    playerScore = 100;
                    playerScoreTextBox.Text = $"Score: {playerScore}/100";
                    StartCountdown();
                    this.Title = $"Mastermind - poging {attempts}/10";
                    codeCracked = false;

                }
                else
                {
                    Application.Current.Shutdown();
                }
            }

        }




        private void highScoreMenu_Click(object sender, RoutedEventArgs e)
        {
           MessageBox.Show(highScores.Clone().ToString());

        }





        private void closeGameMenu_Click(object sender, RoutedEventArgs e)
        {

            // Example: Show confirmation dialog
            var result = MessageBox.Show("Weet je zeker dat je het spel wilt sluiten?", "Bevestigen", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                // Cancel the closing
                return;
            }
            else
            {
                // Perform cleanup or save data
                Application.Current.Shutdown();
            }

        }






        private void numberOfAttemptsMenu_Click(object sender, RoutedEventArgs e)
        {
            string numberOfAttempts = Interaction.InputBox($"Huidige max pogingen: {maxtrys}", "pogingen", "10", 500);
            while (string.IsNullOrEmpty(numberOfAttempts))
            {
                MessageBox.Show("Geef een nummber!", "Foutieve invoer");
                numberOfAttempts = Interaction.InputBox("Geef een nummber", "pogingen", "10", 500);
            }
            maxtrys = numberOfAttempts;

        }





    }
}