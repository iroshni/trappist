import { Component } from '@angular/core';
import { Question } from '../../questions/question.model';
import { Category } from '../../questions/category.model';
import { TestService } from '../tests.service';
import { DifficultyLevel } from '../../questions/enum-difficultylevel';
import { TestQuestion } from '../test-questions/test-question.model';
import { TestCategory } from '../test-sections/test-category.model';

@Component({
    moduleId: module.id,
    selector: 'test-questions',
    templateUrl: 'test-questions.html'
})

export class TestQuestionsComponent {
    editName: boolean;
    TestQuestions: Question[] = [];
    TestQuestion: Question;
    TestQuestionArray: TestQuestion[];
    TestCategory: Category[] = [];
    count: number = 0;
    TestCat: TestCategory;
    result: any;
   
   


    constructor(private testService: TestService) {
        this.TestQuestion = new Question();
        //this.TestQuestions = Array<Question>();
        //this.TestCategory = Array<Category>();
        this.TestQuestionArray = Array<TestQuestion>();
        this.getAllCategory();
        this.getAllQuestions();
        this.TestCat = new TestCategory();
       
       

    }

    private getAllCategory() {
        this.testService.getCategories().subscribe(response => {
            this.TestCategory = response;
            console.log(this.TestCategory);
    });
    }
    private getAllQuestions() {

        this.testService.getQuestions().subscribe(response => {
            this.TestQuestions = response;
            console.log(this.TestQuestions);
        });

    }
    getQuestionLength(x: any) {
        this.count = 0;
        
        return this.TestQuestions.filter(function (y) {
            return y.category.id === x
        }).length;
        
    }
    Count() {
        return this.count += 1;
    }

    toggle(question: Question, category: Category) {
        if (question.isSelect) {
          
        } else {
            question.isSelect = false;       
            }
    }
    postForCheck() {

        var Tests = this.TestQuestions.filter(function (x) {

            return (x.isSelect);

        });
        this.testService.sendQuestions(Tests).subscribe(response => this.result = response);
        console.log(Tests);
  
        

    }
       

    

}
