using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;

namespace rstp_viewer
{
    public partial class Form1 : Form
    {
        private LibVLC _libVLC;
        private List<MediaPlayer> _players;
        private List<VideoView> _videoViews;

        // Lista de tus 4 URLs RTSP
        private string[] rtspUrls = new string[]
        {
            Properties.Settings.Default.RtspCam1,
            Properties.Settings.Default.RtspCam2,
            Properties.Settings.Default.RtspCam3,
            Properties.Settings.Default.RtspCam4
        };

        public Form1()
        {
            InitializeComponent();

            ComboBox viewSelector = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            viewSelector.Items.Add("Ver 1 Cámara");
            viewSelector.Items.Add("Ver 4 Cámaras");
            viewSelector.SelectedIndex = 1; // Por defecto 4 cámaras
            viewSelector.SelectedIndexChanged += (s, e) =>
            {
                bool showFour = viewSelector.SelectedIndex == 1;
                ReloadSettings(showFour);
            };

            Button configButton = new Button
            {
                Text = "Configuración",
                Dock = DockStyle.Top,
                Height = 40
            };

            configButton.Click += (s, e) =>configButton.Click += (s, e) =>
            {
                var settingsForm = new SettingsForm();
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    ReloadSettings(true);
                }
            };

            this.Controls.Add(configButton);

            Core.Initialize();
            _libVLC = new LibVLC();
            _players = new List<MediaPlayer>();
            _videoViews = new List<VideoView>();

            this.Text = "Visualizador de 4 Cámaras RTSP";
            this.WindowState = FormWindowState.Maximized;

            CreateVideoGrid();
            StartStreams();
        }

        private void CreateVideoGrid()
        {
            // Cuadrícula 2x2
            TableLayoutPanel grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 2
            };

            grid.ColumnStyles.Clear();
            grid.RowStyles.Clear();
            for (int i = 0; i < 2; i++)
            {
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            }

            for (int i = 0; i < 4; i++)
            {
                var videoView = new VideoView
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Black
                };
                _videoViews.Add(videoView);
                _players.Add(new MediaPlayer(_libVLC));
                videoView.MediaPlayer = _players[i];
                grid.Controls.Add(videoView, i % 2, i / 2);
            }

            this.Controls.Add(grid);
        }

        private void StartStreams()
        {
            for (int i = 0; i < _players.Count; i++)
            {
                if (i < rtspUrls.Length && !string.IsNullOrWhiteSpace(rtspUrls[i]))
                {
                    try
                    {
                        var media = new Media(_libVLC, rtspUrls[i], FromType.FromLocation);
                        _players[i].Play(media);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al reproducir la cámara {i + 1}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Console.WriteLine($"Cámara {i + 1} no configurada.");
                }
            }
        }


        private void ReloadSettings(bool showFour, int selectedCameraIndex = 0)
        {
            foreach (var player in _players)
            {
                player.Stop();
                player.Dispose();
            }

            _players.Clear();
            _videoViews.Clear();
            this.Controls.Clear();

            // Vuelve a cargar las URLs
            rtspUrls = new string[]
            {
        Properties.Settings.Default.RtspCam1,
        Properties.Settings.Default.RtspCam2,
        Properties.Settings.Default.RtspCam3,
        Properties.Settings.Default.RtspCam4
            };

            if (showFour)
                CreateVideoGrid();
            else
                CreateSingleView(selectedCameraIndex);

            StartStreams();

            // Volver a agregar controles
            AddTopControls(showFour);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (var player in _players)
            {
                player.Stop();
                player.Dispose();
            }

            base.OnFormClosing(e);
        }

        private void CreateSingleView(int cameraIndex)
        {
            var videoView = new VideoView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            _videoViews.Add(videoView);
            var player = new MediaPlayer(_libVLC);
            videoView.MediaPlayer = player;

            _players.Add(player);
            this.Controls.Add(videoView);

            // Validar si la cámara existe y tiene URL
            if (cameraIndex >= 0 && cameraIndex < rtspUrls.Length && !string.IsNullOrWhiteSpace(rtspUrls[cameraIndex]))
            {
                try
                {
                    var media = new Media(_libVLC, rtspUrls[cameraIndex], FromType.FromLocation);
                    player.Play(media);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al reproducir la cámara {cameraIndex + 1}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("La cámara seleccionada no tiene una URL válida.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void AddTopControls(bool currentViewIsFour)
        {
            // ComboBox
            ComboBox viewSelector = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            viewSelector.Items.Add("Ver 1 Cámara");
            viewSelector.Items.Add("Ver 4 Cámaras");
            viewSelector.SelectedIndex = currentViewIsFour ? 1 : 0;
            viewSelector.SelectedIndexChanged += (s, e) =>
            {
                bool showFour = viewSelector.SelectedIndex == 1;
                ReloadSettings(showFour);
            };

            // Botón Configuración
            Button configButton = new Button
            {
                Text = "Configuración",
                Dock = DockStyle.Top,
                Height = 40
            };

            configButton.Click += (s, e) =>
            {
                var settingsForm = new SettingsForm();
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    ReloadSettings(currentViewIsFour);
                }
            };

            // Agregar a la interfaz
            this.Controls.Add(configButton);
            this.Controls.Add(viewSelector);
            this.Controls.SetChildIndex(viewSelector, 0);
            this.Controls.SetChildIndex(configButton, 1);

            ComboBox cameraSelector = new ComboBox
            {
                Name = "CameraSelector",
                Dock = DockStyle.Top,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = !currentViewIsFour // Solo visible en modo 1 cámara
            };

            cameraSelector.Items.Add("Cámara 1");
            cameraSelector.Items.Add("Cámara 2");
            cameraSelector.Items.Add("Cámara 3");
            cameraSelector.Items.Add("Cámara 4");
            cameraSelector.SelectedIndex = 0;

            cameraSelector.SelectedIndexChanged += (s, e) =>
            {
                ReloadSettings(false, cameraSelector.SelectedIndex);
            };

            this.Controls.Add(cameraSelector);
            this.Controls.SetChildIndex(cameraSelector, 0);


        }


    }
}
