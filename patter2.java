package com.k7it.Assignment2;

import java.util.Scanner;

public class patter2 {

	public static void main(String[] args) {

		Scanner sc = new Scanner(System.in);
		System.out.println("enter n value");
		int n = sc.nextInt();
		for (int i = 1; i <= n; i++) {
			for (int j = 1; j <= n; j++) {
				if (i == 1 || i == n || j == 1 || j == n || i==j || i+j==n+1 /*j==i-2 ||j==i+2*/) {
					System.out.print("* ");
				}
				else {
					System.out.print("  ");
				}
			}
			System.out.println();
		}
	}
}
