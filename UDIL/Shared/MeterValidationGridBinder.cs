using System.Data;
using System.Web.UI.WebControls;

namespace UDIL.Shared.Web
{
    /// <summary>
    /// Binds the three standard meter validation GridViews only when data changed.
    /// </summary>
    public static class MeterValidationGridBinder
    {
        public static void BindIfChanged(MeterTablesRefreshResult refresh, GridView meterVisuals, GridView communicationHistory, GridView events)
        {
            if (refresh == null || refresh.SkippedQuery || !refresh.DataChanged || refresh.DataSet == null)
            {
                return;
            }

            BindTable(refresh.DataSet, "MeterVisuals", meterVisuals);
            BindTable(refresh.DataSet, "CommunicationHistory", communicationHistory);
            BindTable(refresh.DataSet, "Events", events);
        }

        public static void ForceBind(DataSet ds, GridView meterVisuals, GridView communicationHistory, GridView events)
        {
            BindTable(ds, "MeterVisuals", meterVisuals);
            BindTable(ds, "CommunicationHistory", communicationHistory);
            BindTable(ds, "Events", events);
        }

        private static void BindTable(DataSet ds, string tableName, GridView grid)
        {
            if (grid == null)
            {
                return;
            }

            if (ds != null && ds.Tables.Contains(tableName) && ds.Tables[tableName].Rows.Count > 0)
            {
                var table = ds.Tables[tableName];

                // Ensure any BoundFields declared in the GridView that reference
                // columns not present in the DataTable are created with empty values.
                // This prevents HttpException: "A field or property with the name 'X' was not found..."
                foreach (DataControlField field in grid.Columns)
                {
                    if (field is BoundField bf)
                    {
                        string dataField = bf.DataField;
                        if (!string.IsNullOrEmpty(dataField) && !table.Columns.Contains(dataField))
                        {
                            table.Columns.Add(new System.Data.DataColumn(dataField, typeof(string)));
                            // populate empty values for existing rows
                            foreach (System.Data.DataRow row in table.Rows)
                            {
                                row[dataField] = string.Empty;
                            }
                        }
                    }
                }

                grid.DataSource = table;
            }
            else
            {
                grid.DataSource = null;
            }

            grid.DataBind();
        }
    }
}
