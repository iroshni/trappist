import { Test } from '../tests.model';
import { Category } from '../../questions/category.model';



export class TestCategory {
 
    Id: number;
    TestId: Test;      
    CategoryId: Category;
    numberOfQuestion: number;

}