public class Position
{
    public int  Row {get; }
    public int  Column {get; }


    public Position (int row, int Columnn){

        Row = row;
        Column = Columnn;

    }

    public override bool Equals(object a){

        if (a is Position other){

            return Row == other.Row && Column == other.Column;
        }

        return false;
    }

    public override int GetHashCode(){
        return 8 * Row + Column;
    }
}
