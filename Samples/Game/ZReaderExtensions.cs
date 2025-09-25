namespace ZZTGame;

public static class ZReaderExtensions
{
    public static Color GetColor(int data)
    {
        return data switch
        {
            0 => Color.AnsiBlack,
            1 => Color.AnsiBlue,
            2 => Color.AnsiGreen,
            3 => Color.AnsiCyan,
            4 => Color.AnsiRed,
            5 => Color.AnsiMagenta,
            6 => Color.AnsiYellow,
            7 => Color.AnsiWhite,
            8 => Color.AnsiBlackBright,
            9 => Color.AnsiBlueBright,
            10 => Color.AnsiGreenBright,
            11 => Color.AnsiCyanBright,
            12 => Color.AnsiRedBright,
            13 => Color.AnsiMagentaBright,
            14 => Color.AnsiYellowBright,
            15 => Color.AnsiWhiteBright,
            _ => Color.CornflowerBlue
        };
    }
}
