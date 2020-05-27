package code;

public enum FSP{

    E_ERROR(0),
    S_SUCCESS(1),
    R_SENT_FILE(2),
    R_CREATE_DIRECTORY(3),
    R_TREE_DIRECTORY(4),
    R_REMOVE(5),
    R_FILE(6),
    C_CLOSE_CONECTION(7);

    private final int value;

    FSP(int value){
        this.value = value;
    }

    public int getVal(){
        return value;
    }

}
