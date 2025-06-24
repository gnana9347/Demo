package com.k7it.pack1;

/*public enum WeekDays {
	SUNDAY,MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY,SATURDAY;
	
	private WeekDays() {
		System.out.println("NO-ARG");
	}

	public static void main(String[] args) {
		WeekDays w1=WeekDays.FRIDAY;
		System.out.println(w1);
	}
}*/

public enum WeekDays {
	SUNDAY(),MONDAY(2),TUESDAY(3),WEDNESDAY(4),THURSDAY(5),FRIDAY(6),SATURDAY(7);
	
	private WeekDays(int i) {
		System.out.println(i);
	}
	private WeekDays() {
		System.out.println("NO-ARG");
	}

	public static void main(String[] args) {
		WeekDays w1=WeekDays.FRIDAY;
		System.out.println(w1);
	}
}
