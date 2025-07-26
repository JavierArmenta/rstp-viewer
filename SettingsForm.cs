using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rstp_viewer
{
    public partial class SettingsForm : Form
    {
        TextBox[] textBoxes;
        public SettingsForm()
        {
            InitializeComponent();
            this.Text = "Configuración de Cámaras";
            this.Width = 500;
            this.Height = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            ComboBox viewSelector = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };


            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 5,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));

            textBoxes = new TextBox[4];
            for (int i = 0; i < 4; i++)
            {
                table.Controls.Add(new Label { Text = $"Cámara {i + 1}:", TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, i);

                var tb = new TextBox { Dock = DockStyle.Fill };
                tb.Text = GetCameraUrl(i);
                textBoxes[i] = tb;
                table.Controls.Add(tb, 1, i);
            }

            var btnGuardar = new Button
            {
                Text = "Guardar",
                Dock = DockStyle.Fill
            };
            btnGuardar.Click += BtnGuardar_Click;

            table.Controls.Add(btnGuardar, 0, 4);
            table.SetColumnSpan(btnGuardar, 2);

            this.Controls.Add(table);
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.RtspCam1 = textBoxes[0].Text.Trim();
            Properties.Settings.Default.RtspCam2 = textBoxes[1].Text.Trim();
            Properties.Settings.Default.RtspCam3 = textBoxes[2].Text.Trim();
            Properties.Settings.Default.RtspCam4 = textBoxes[3].Text.Trim();

            Properties.Settings.Default.Save();

            MessageBox.Show("Configuración guardada.", "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetCameraUrl(int index)
        {
            return index switch
            {
                0 => Properties.Settings.Default.RtspCam1,
                1 => Properties.Settings.Default.RtspCam2,
                2 => Properties.Settings.Default.RtspCam3,
                3 => Properties.Settings.Default.RtspCam4,
                _ => ""
            };
        }
    }
}
