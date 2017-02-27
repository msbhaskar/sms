// #@ assembly name="$(TargetDir)$(TargetFileName)" #


 

/// <reference path="Enums.ts" />

declare module StudentManagementSystem.Data.ViewModels {
	interface JsonReturnViewModel<T> {
		element: T;
		errormessage: string;
		haserror: boolean;
	}
}
declare module StudentManagementSystem.Data.ViewModels.Schools {
	interface CourseSchedule {
		course_id: string;
		Day: string;
		Time: string;
	}
	interface SchoolAddress {
		Building: string;
		City: string;
		Link: string[];
		State: string;
		Street: string;
		ZipCode: string;
	}
	interface SchoolViewModel {
		Address: StudentManagementSystem.Data.ViewModels.Schools.SchoolAddress;
		Courses: StudentManagementSystem.Data.ViewModels.Schools.CourseSchedule[];
		Id: string;
		Name: string;
		StartDate: Date;
	}
}


