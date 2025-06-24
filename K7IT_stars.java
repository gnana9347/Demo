package com.k7it.Assignment2;

import java.util.Scanner;

public class k7IT_stars
 {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        System.out.println("Enter n value");
        int n = sc.nextInt();

        for (int i = 1; i <= n; i++) {
            // Pattern 1
            for (int j = 1; j <= n; j++) {
                if (j == 1 || i + j == n || j == i - 1) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            System.out.print("   "); 

            // Pattern 2
            for (int j = 1; j <= n; j++) {
                if (i == 1 || j == n) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            System.out.print("   "); 

            // Pattern 3
            for (int j = 1; j <= n; j++) {
                if (i == 1 || i == n || j == 3) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            System.out.print("   "); 

            // Pattern 4
            for (int j = 1; j <= n; j++) {
                if (i == 1 || j == 3) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            System.out.print("   "); 

            // Pattern 5
            for (int j = 1; j <= n; j++) {
                if (i == 1 || j == 1 || i == n || i == 3) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            System.out.print("   ");

            // Pattern 6
            for (int j = 1; j <= n; j++) {
                if (i == 1 || j == 1 || i == n) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            System.out.print("   "); 
            
            for (int j = 1; j <= n; j++) {
                if (j==1 || j==n || i==3) {
                    System.out.print("* ");
                } else {
                    System.out.print("  ");
                }
            }
            
            
            System.out.println();
        }
    }
}
