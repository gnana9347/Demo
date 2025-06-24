package com.k7it.repo;

import org.springframework.data.mongodb.repository.MongoRepository;

import com.k7it.model.Visitor;

public interface VisitorRepo extends MongoRepository<Visitor, String> {

}
