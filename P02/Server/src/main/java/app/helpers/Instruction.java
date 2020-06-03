package app.helpers;

public enum Instruction {
    DISCONNECT(0),
    OK(1),
    REQUEST_DB(2),
    REQUEST_IMAGES(3),
    REQUEST_ONE_IMAGE(4),
    FINISHED_PROCESS(5),
    POST_PURCHASE(6);

    private final int value;
    private Instruction (int value) {
        this.value = value;
    }

    public int getValue() {
        return value;
    }
}
