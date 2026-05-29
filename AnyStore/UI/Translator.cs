using System.Windows.Forms;

namespace AnyStore.UI
{
    public static class Translator
    {
        public static void TranslateGrid(DataGridView dgv)
        {
            if (dgv == null || dgv.Columns == null) return;

            if (dgv.Columns.Contains("id"))
                dgv.Columns["id"].Visible = false;
            else if (dgv.Columns.Count > 0 && dgv.Columns[0].Name.ToLower() == "id")
                dgv.Columns[0].Visible = false;

            if (!dgv.Columns.Contains("ItemNo"))
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = "ItemNo";
                col.HeaderText = "N°";
                col.ReadOnly = true;
                col.DisplayIndex = 0;
                dgv.Columns.Add(col);
            }

            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Cells["ItemNo"].Value = (i + 1).ToString();
            }

            foreach (DataGridViewColumn col in dgv.Columns)
            {
                switch (col.Name.ToLower())
                {
                    case "id": col.HeaderText = "Codigo"; break;
                    case "name": col.HeaderText = "Nombre"; break;
                    case "category": col.HeaderText = "Categoria"; break;
                    case "description": col.HeaderText = "Descripcion"; break;
                    case "qty": col.HeaderText = "Cantidad"; break;
                    case "added_date": col.HeaderText = "Anadido el"; break;
                    case "added_by": col.HeaderText = "Anadido por"; break;
                    case "first_name": col.HeaderText = "Nombres"; break;
                    case "last_name": col.HeaderText = "Apellidos"; break;
                    case "email": col.HeaderText = "Correo"; break;
                    case "username": col.HeaderText = "Usuario"; break;
                    case "password": col.HeaderText = "Contrasena"; break;
                    case "contact": col.HeaderText = "Telefono"; break;
                    case "address": col.HeaderText = "Direccion"; break;
                    case "gender": col.HeaderText = "Genero"; break;
                    case "user_type": col.HeaderText = "Tipo de Usuario"; break;
                    case "title": col.HeaderText = "Titulo"; break;
                }
            }
        }
    }
}
