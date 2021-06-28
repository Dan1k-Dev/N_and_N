using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Подключение базы данных 
        /// </summary>
        SoccerContext db;

        /// <summary>
        /// Конструктор
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            db = new SoccerContext();
            db.Players.Load();
            dataGridView1.DataSource = db.Players.Local.ToBindingList();
        }

       /// <summary>
       /// Добавление данных
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            PlayerForm plForm = new PlayerForm();
            /// <summary>
            /// добавляем список команд на форму plForm
            /// </summary>
            List<Team> teams = db.Teams.ToList();
            plForm.listBox1.DataSource = teams;
            plForm.listBox1.ValueMember = "Id";
            plForm.listBox1.DisplayMember = "Name";

            DialogResult result = plForm.ShowDialog(this);
            if (result == DialogResult.Cancel)
                return;

            Player player = new Player();
            player.Age = (int)plForm.numericUpDown1.Value;
            player.Name = plForm.textBox1.Text;

            /// <summary>
            /// Очищаем список и заново заполняем его выделенными элементами
            /// </summary>
            teams.Clear(); 
            foreach (var item in plForm.listBox1.SelectedItems)
            {
                teams.Add((Team)item);
            }
            player.Teams = teams;
            db.Players.Add(player);
            db.SaveChanges();

            MessageBox.Show("Новый игрок добавлен");
        }

        /// <summary>
        /// Редактирование
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1)
                return;

            int index = dataGridView1.SelectedRows[0].Index;
            int id = 0;
            bool converted = Int32.TryParse(dataGridView1[0, index].Value.ToString(), out id);
            if (converted == false)
                return;

            Player player = db.Players.Find(id);

            PlayerForm plForm = new PlayerForm();
            plForm.numericUpDown1.Value = player.Age;
            plForm.textBox1.Text = player.Name;

            /// <summary>
            /// Получение списка команд 
            /// </summary>           
            List<Team> teams = db.Teams.ToList();
            plForm.listBox1.DataSource = teams;
            plForm.listBox1.ValueMember = "Id";
            plForm.listBox1.DisplayMember = "Name";
            foreach (Team t in player.Teams)
                plForm.listBox1.SelectedItem = t;

            DialogResult result = plForm.ShowDialog(this);
            if (result == DialogResult.Cancel)
                return;

            player.Age = (int)plForm.numericUpDown1.Value;
            player.Name = plForm.textBox1.Text;

            ///<summary>
            /// Проверка наличия команд у игрока
            ///</summary>          
            foreach (var team in teams)
            {
                if (plForm.listBox1.SelectedItems.Contains(team))
                {
                    if (!player.Teams.Contains(team))
                        player.Teams.Add(team);
                }
                else
                {
                    if (player.Teams.Contains(team))
                        player.Teams.Remove(team);
                }
            }
            db.Entry(player).State = EntityState.Modified;
            db.SaveChanges();
            MessageBox.Show("Информация обновлена");
        }

        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count < 1)
                return;

            int index = dataGridView1.SelectedRows[0].Index;
            int id = 0;
            bool converted = Int32.TryParse(dataGridView1[0, index].Value.ToString(), out id);
            if (converted == false)
                return;

            Player player = db.Players.Find(id);

            db.Players.Remove(player);
            db.SaveChanges();

            MessageBox.Show("Футболист удален");
        }

        /// <summary>
        /// Добавление новой команды
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            TeamForm tmForm = new TeamForm();

            DialogResult result = tmForm.ShowDialog(this);
            if (result == DialogResult.Cancel)
                return;

            Team team = new Team();
            team.Name = tmForm.textBox1.Text;
            team.Coach = tmForm.textBox2.Text;
            team.Players = new List<Player>();

            db.Teams.Add(team);
            db.SaveChanges();
            MessageBox.Show("Новая команда добавлена");
        }
    }


    
}
