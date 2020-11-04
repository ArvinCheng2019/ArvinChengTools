using System;


public abstract class ListValueViewBase
{

    public int Count
    {
        get
        {
            return this._size;
        }
    }


    protected const int DefaultCapacity = 4;


    protected int _size;


    protected int _version;
}
