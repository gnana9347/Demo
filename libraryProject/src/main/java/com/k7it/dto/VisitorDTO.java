package com.k7it.dto;

import org.springframework.web.multipart.MultipartFile;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;
import lombok.Data;

@Data
public class VisitorDTO {

	@NotBlank(message = "Name is mandatory field")
	@Size(min = 3, max = 50, message = "Name must be min 3 chars and max 50 chars")
	private String name;

	@NotNull(message = "Emil is mandatory field")
	@Email(message = "invalid emial id")
	private String email;

	@Pattern(regexp = "^\\+?[0-9]{10,15}$", message = "Phone number is invalid. It should be 10 to 15 digits long and may start with a +.")
	private String phone;

	@NotNull(message = "Age is mandatory field")
	@Min(value = 0)
	@Max(value = 100)
	private Integer age;

	@NotNull(message = "aadhar is mandatory field")
	private MultipartFile aadhar;

	@NotNull(message = "Photo is mandatory field")
	private MultipartFile photo;

	
}

