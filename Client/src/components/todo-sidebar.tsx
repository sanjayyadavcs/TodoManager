import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Briefcase, User, List, Plus } from "lucide-react";
import { cn } from "@/lib/utils";

interface TodoStats {
  total: number;
  work: number;
  personal: number;
  completed: number;
  highPriority: number;
  mediumPriority: number;
  lowPriority: number;
}

interface TodoSidebarProps {
  stats?: TodoStats;
  selectedCategory: string;
  selectedPriority: string;
  onCategorySelect: (category: string) => void;
  onPrioritySelect: (priority: string) => void;
  onCreateTodo: () => void;
}

export default function TodoSidebar({
  stats,
  selectedCategory,
  selectedPriority,
  onCategorySelect,
  onPrioritySelect,
  onCreateTodo,
}: TodoSidebarProps) {
  return (
    <div className="lg:col-span-1">
      <Card>
        <CardContent className="p-6">
          <h2 className="text-lg font-semibold text-primary-custom mb-6">Categories & Filters</h2>
          
          {/* Category Filters */}
          <div className="space-y-3 mb-6">
            <Button
              variant={selectedCategory === "all" ? "default" : "outline"}
              className={cn(
                "w-full justify-between",
                selectedCategory === "all" 
                  ? "bg-primary text-white hover:bg-primary/90" 
                  : "bg-gray-100 text-primary-custom hover:bg-gray-200"
              )}
              onClick={() => onCategorySelect("all")}
            >
              <span className="flex items-center space-x-2">
                <List className="h-4 w-4" />
                <span>All Todos</span>
              </span>
              <span className={cn(
                "px-2 py-1 rounded-full text-xs",
                selectedCategory === "all" 
                  ? "bg-white bg-opacity-20" 
                  : "bg-gray-300"
              )}>
                {stats?.total || 0}
              </span>
            </Button>
            
            <Button
              variant={selectedCategory === "work" ? "default" : "outline"}
              className={cn(
                "w-full justify-between",
                selectedCategory === "work" 
                  ? "bg-primary text-white hover:bg-primary/90" 
                  : "bg-gray-100 text-primary-custom hover:bg-gray-200"
              )}
              onClick={() => onCategorySelect("work")}
            >
              <span className="flex items-center space-x-2">
                <Briefcase className="h-4 w-4 text-blue-600" />
                <span>View Work</span>
              </span>
              <span className={cn(
                "px-2 py-1 rounded-full text-xs",
                selectedCategory === "work" 
                  ? "bg-white bg-opacity-20" 
                  : "bg-gray-300"
              )}>
                {stats?.work || 0}
              </span>
            </Button>
            
            <Button
              variant={selectedCategory === "personal" ? "default" : "outline"}
              className={cn(
                "w-full justify-between",
                selectedCategory === "personal" 
                  ? "bg-primary text-white hover:bg-primary/90" 
                  : "bg-gray-100 text-primary-custom hover:bg-gray-200"
              )}
              onClick={() => onCategorySelect("personal")}
            >
              <span className="flex items-center space-x-2">
                <User className="h-4 w-4 text-green-600" />
                <span>View Personal Todo</span>
              </span>
              <span className={cn(
                "px-2 py-1 rounded-full text-xs",
                selectedCategory === "personal" 
                  ? "bg-white bg-opacity-20" 
                  : "bg-gray-300"
              )}>
                {stats?.personal || 0}
              </span>
            </Button>
          </div>

          {/* Priority Filters */}
          <div className="border-t border-gray-200 pt-6">
            <h3 className="text-sm font-medium text-secondary-custom mb-3">Priority Levels</h3>
            <div className="space-y-2">
              <Button
                variant="ghost"
                className={cn(
                  "w-full justify-between text-sm h-auto py-2",
                  selectedPriority === "high" && "bg-gray-100"
                )}
                onClick={() => onPrioritySelect(selectedPriority === "high" ? "" : "high")}
              >
                <span className="flex items-center space-x-2">
                  <div className="w-3 h-3 bg-priority-high rounded-full"></div>
                  <span>High Priority</span>
                </span>
                <span className="text-secondary-custom">{stats?.highPriority || 0}</span>
              </Button>
              
              <Button
                variant="ghost"
                className={cn(
                  "w-full justify-between text-sm h-auto py-2",
                  selectedPriority === "medium" && "bg-gray-100"
                )}
                onClick={() => onPrioritySelect(selectedPriority === "medium" ? "" : "medium")}
              >
                <span className="flex items-center space-x-2">
                  <div className="w-3 h-3 bg-priority-medium rounded-full"></div>
                  <span>Medium Priority</span>
                </span>
                <span className="text-secondary-custom">{stats?.mediumPriority || 0}</span>
              </Button>
              
              <Button
                variant="ghost"
                className={cn(
                  "w-full justify-between text-sm h-auto py-2",
                  selectedPriority === "low" && "bg-gray-100"
                )}
                onClick={() => onPrioritySelect(selectedPriority === "low" ? "" : "low")}
              >
                <span className="flex items-center space-x-2">
                  <div className="w-3 h-3 bg-priority-low rounded-full"></div>
                  <span>Low Priority</span>
                </span>
                <span className="text-secondary-custom">{stats?.lowPriority || 0}</span>
              </Button>
            </div>
          </div>

          {/* Add New Todo Button */}
          <div className="border-t border-gray-200 pt-6">
            <Button
              className="w-full bg-green-600 text-white hover:bg-green-700"
              onClick={onCreateTodo}
            >
              <Plus className="h-4 w-4 mr-2" />
              Add New Todo
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
