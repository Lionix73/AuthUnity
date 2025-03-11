using System;

[Serializable]
public class UserModel
{
    public string _id;
    public string username;
    public bool estado;
    public DataUser data;
}

[Serializable]
public class DataUser
{
    public int score;
}

[Serializable]
public class UserList
{
    public UserModel[] usuarios;
}
