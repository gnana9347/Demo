
import java.io.*;
import java.util.*;
import java.text.*;
import java.math.*;
import java.util.regex.*;
import java.util.Arrays;
import java.util.List;
 
import java.util.stream.*;
 
class PasswordSanitizer{
    public String filter(List<String> passwords){
        String newStr = passwords.stream().filter((str) -> {
            if (isTooShort(str)) {
                return false;
            }
            if (isOnlyChar(str)) {
                return false;
            }
            if (isOnlyDigit(str)) {
                return false;
            }
            return true;
        }).collect(Collectors.joining(" "));
        return newStr; //implement it, and return space separated passwords
    }
    public static boolean isTooShort (String str) {
        if (str.length() < 5) {
            return true;
        }
        return false;
    }
    public static boolean isOnlyChar (String str) {
        if (str.matches("[a-zA-Z]+")) {
            return true;
        }
        return false;
    }
    public static boolean isOnlyDigit (String str) {
        if (str.matches("[0-9]+")) {
            return true;
        }
        return false;
    }
}
public class Solution {
    public static void main(String args[] ) throws Exception {
        //reader and writer
        BufferedReader bufferedReader = new BufferedReader(new InputStreamReader(System.in));
        BufferedWriter bufferedWriter = new BufferedWriter(new FileWriter(System.getenv("OUTPUT_PATH")));
 
        String[] passwords = bufferedReader.readLine().split(" ");
        PasswordSanitizer ps = new PasswordSanitizer();
        String output = ps.filter(Arrays.asList(passwords));
        bufferedWriter.write(output);
        bufferedWriter.newLine();;
        bufferedReader.close();
        bufferedWriter.close();
    }   
}