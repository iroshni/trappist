import { Test } from '../tests.model';
import { Question } from '../../questions/question.model';
import { TestCategory } from '../test-sections/test-category.model';


export class TestQuestion {
    
    id: number;
    testId: Test;     
    questionId: Question;
    testCategoryId: TestCategory;
 

}