﻿using SportsComplex.Entities;
using SportsComplex.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SportsComplex.DesktopUI
{
    public partial class MainForm : Form
    {
        public MainForm(User user)
        {
            InitializeComponent();
            _rentsRepository = new SqlRentsRepository(connString);
            this.Text = String.Format("{0} - [Logged as {1} {2}]", this.Text, user.FirstName, user.LastName);
        }

        private void miShowPriceList_Click(object sender, EventArgs e)
        {
            PriceListForm frmPriceList = new PriceListForm();
            frmPriceList.ShowDialog();
        }

        private void miExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Load current rents.
            var rents = _rentsRepository.GetRentsOnDate(DateTime.Now);

            // Update UI.
            UpdateRentsDataGridView(rents);
        }

        public void UpdateRentsDataGridView(IEnumerable<RentItem> rents)
        {
            dgvRents.Rows.Clear();

            foreach (var r in rents)
            {
                dgvRents.Rows.Add(
                    r.Id, 
                    string.Format("{0} {1}.", r.Renter.LastName, r.Renter.FirstName[0]), 
                    r.SportsHall.Type.Name, 
                    r.SportsHall.Area,
                    r.SportsHall.Rate,
                    r.DateStart,
                    r.DateEnd,
                    r.Renter.Phone,
                    r.Cost);
            }
        }

        private void sportsHallsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SportsHallsForm frmSportsHalls = new SportsHallsForm();
            frmSportsHalls.ShowDialog();
        }

        private void btnCancelRent_Click(object sender, EventArgs e)
        {
            if (dgvRents.Rows.Count > 0 && dgvRents.SelectedRows[0].Index >= 0)
            {
                var dr = MessageBox.Show("Do you really want to finish the rent?", "Finishing rent", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (dr == DialogResult.Yes)
                {
                    decimal moneyChange = _rentsRepository.Remove((int) dgvRents.SelectedRows[0].Cells[0].Value);

                    if (moneyChange > 0)
                    {
                        MessageBox.Show(String.Format("Change: {0}", moneyChange), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    UpdateRentsDataGridView(_rentsRepository.GetRentsOnDate(DateTime.Now));
                }
            }
        }

        private void btnNewRent_Click(object sender, EventArgs e)
        {
            NewRentForm frmNewRent = new NewRentForm();

            if (frmNewRent.ShowDialog() == DialogResult.OK)
            {
                UpdateRentsDataGridView(_rentsRepository.GetRentsOnDate(DateTime.Now));
            }
        }

        private void cbRenter_DropDown(object sender, EventArgs e)
        {
            cbRenter.Items.Clear();

            var rents = _rentsRepository.GetRentsOnDate(DateTime.Now);
            var orderedRents = from r in rents
                        orderby r.Renter.LastName, r.Renter.FirstName
                        select r;
            var distincRents = orderedRents
                                .GroupBy(a => a.Renter.Id)
                                .Select(g => g.First());

            foreach (var renter in distincRents)
            {
                cbRenter.Items.Add(renter.Renter);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            cbRenter.Items.Clear();
            dtpDate.Value = DateTime.Now;
            UpdateRentsDataGridView(_rentsRepository.GetRentsOnDate(DateTime.Now));
        }

        private void cbRenter_SelectedIndexChanged(object sender, EventArgs e)
        {
            var rents = _rentsRepository.GetRentsByRenterId(((Renter)(cbRenter.SelectedItem)).Id);

            UpdateRentsDataGridView(rents);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm frmAbout = new AboutForm();
            frmAbout.ShowDialog();
        }

        private string connString = ConfigurationManager.ConnectionStrings["SportsComplexConnectionString"].ConnectionString;
        // Repositories.
        private SqlRentsRepository _rentsRepository;
        private SqlRentersRepository _rentersRepository;

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            var rents = _rentsRepository.GetRentsOnDate(dtpDate.Value);

            UpdateRentsDataGridView(rents);
        }
    }
}
