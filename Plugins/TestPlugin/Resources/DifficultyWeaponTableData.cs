using FrostySdk.IO;
using FrostySdk.Resources;
using System.Collections.Generic;

namespace TestPlugin.Resources
{
    // Classes that derive from EbxAsset are used to specify any custom handling of ebx related data, through the use of a
    // specialized ModifiedResource object. Refer to App.AssetManager.GetEbxAs<T> to obtain the ebx data
    public class DifficultyWeaponTableData : EbxAsset
    {
        // holds the modified data for this ebx asset
        private ModifiedDifficultyWeaponTableData modified;

        // invoked during asset load to give the asset a chance to perform any logic required
        // when loading the modified data
        public override void ApplyModifiedResource(ModifiedResource modifiedResource)
        {
            // obtains the modified data
            modified = modifiedResource as ModifiedDifficultyWeaponTableData;

            // applies the modified data to the ebx data
            dynamic rootTable = RootObject;
            foreach (var value in modified.Values)
            {
                rootTable.WeaponTable[value.Row].Values[value.Column].Value = value.Value;
            }
        }

        // invoked during asset save to save the modified data
        public override ModifiedResource SaveModifiedResource()
        {
            return modified;
        }

        public void ModifyValue(int row, int column, float value)
        {
            // create a new modified object if necessary
            if (modified == null)
                modified = new ModifiedDifficultyWeaponTableData();

            // pass call over to modified object
            modified.ModifyValue(row, column, value);
        }
    }

    // ModifiedResource and any class that derives from it are the classes used when it is required to store
    // data in a serializable fashion as opposed to a regular ebx/res byte array, it provides its own Read/Write
    // functions, and allows for the user to store any data they see fit, as long as they can apply it onto
    // an asset/resource.
    public class ModifiedDifficultyWeaponTableData : ModifiedResource
    {
        public IEnumerable<ColumnRowValue> Values => values;

        // stores modifications as a series of row/column/value sets
        private List<ColumnRowValue> values = new List<ColumnRowValue>();

        public void ModifyValue(int row, int column, float value)
        {
            // check to see if row/column set already exists
            int index = values.FindIndex((ColumnRowValue crv) => crv.Row == row && crv.Column == column);
            if (index == -1)
            {
                // if not, then create it
                values.Add(new ColumnRowValue() { Column = column, Row = row });
                index = values.Count - 1;
            }

            values[index].Value = value;
        }

        // This function is responsible for reading in the modified data from the project file
        public override void ReadInternal(NativeReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                ColumnRowValue crv = new ColumnRowValue
                {
                    Row = reader.ReadInt(),
                    Column = reader.ReadInt(),
                    Value = reader.ReadFloat()
                };
                values.Add(crv);
            }
        }

        // This function is responsible for writing out the modified data to the project file
        public override void SaveInternal(NativeWriter writer)
        {
            writer.Write(values.Count);
            foreach (var value in values)
            {
                writer.Write(value.Row);
                writer.Write(value.Column);
                writer.Write(value.Value);
            }
        }
    }

    #region -- DifficultyWeaponTableData objects --

    public class ColumnRowValue
    {
        public int Column;
        public int Row;
        public float Value;
    }

    #endregion
}
