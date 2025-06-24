package com.k7it.homeController;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.ui.Model;

@Controller
public class HomeController {

    @GetMapping("/")
    public String home() {
        return "home";
    }

    @GetMapping("/book-info/{id}")
    public String getBookInfo(@PathVariable String id, Model model) {
        // Retrieve the book information using the ID (replace with your actual service method)
        // For simplicity, I'm using a placeholder book model. Replace with actual service to fetch details.

        // Assuming you have a BookService to fetch book details by id
        // Book book = bookService.getBookById(id);
        // model.addAttribute("book", book);

        // Dummy book data (replace with real data)
        model.addAttribute("bookId", id);
        model.addAttribute("bookTitle", "Sample Book Title");
        model.addAttribute("bookDescription", "This is a description of the sample book.");

        return "book_info"; // This will map to the book_info.html page
    }
    
    @GetMapping("/sign-up")
    public String signUpPage() {
    	return "sign_up";
    }
    @GetMapping("/create-visitor")
    public String createVisitorPage() {
        return "create_visitor"; // Corresponds to src/main/resources/templates/create_visitor.html
    }

    @GetMapping("/list-visitors")
    public String listVisitorPage() {
        return "list_visitors"; // Corresponds to src/main/resources/templates/list_visitors.html
    }

    @GetMapping("/edit-visitor")
    public String editVisitorPage() {
        return "edit_visitor"; // Corresponds to src/main/resources/templates/edit_visitor.html
    }

    @GetMapping("/view-visitor")
    public String viewVisitorPage() {
        return "view_visitor"; // Corresponds to src/main/resources/templates/view_visitor.html
    }
}
