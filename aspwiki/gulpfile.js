"use strict";

var project = require('./project.json');

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    less = require("gulp-less");

gulp.watch('Styles/*.less', ["less"]);

gulp.task("less", function () {
    return gulp.src('Styles/**/*.less')
        .pipe(concat('main.css'))
        .pipe(less())
        .pipe(gulp.dest(project.webroot + '/css'));
});